using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Proyecto_Dnd.Database.MongoDB.Models;

namespace Proyecto_Dnd.Database.MongoDB.Services
{
    public class CombatLogService
    {
        private readonly IMongoCollection<CombatLog> _collection;

        public CombatLogService()
        {
            var db = MongoDBConfig.GetDatabase();
            _collection = db.GetCollection<CombatLog>("combat_logs");
        }

        /// <summary>
        /// Guarda un combate completo en MongoDB
        /// </summary>
        public void GuardarCombate(CombatLog combate)
        {
            try
            {
                combate.Fecha = DateTime.Now;
                _collection.InsertOne(combate);

                System.Diagnostics.Debug.WriteLine($"✅ Combate registrado: {combate.JugadorNombre} vs {combate.EnemigoNombre} - {combate.Resultado}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error guardando combate: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene el historial de combates de un jugador
        /// </summary>
        public List<CombatLog> ObtenerCombatesJugador(int jugadorId, int limite = 50)
        {
            try
            {
                return _collection.Find(c => c.JugadorId == jugadorId)
                                  .SortByDescending(c => c.Fecha)
                                  .Limit(limite)
                                  .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error obteniendo combates: {ex.Message}");
                return new List<CombatLog>();
            }
        }

        /// <summary>
        /// Obtiene estadísticas de combate de un jugador
        /// </summary>
        public EstadisticasCombate ObtenerEstadisticas(int jugadorId)
        {
            try
            {
                var combates = ObtenerCombatesJugador(jugadorId, 1000);

                var stats = new EstadisticasCombate
                {
                    TotalCombates = combates.Count,
                    Victorias = combates.FindAll(c => c.Resultado == "victoria").Count,
                    Derrotas = combates.FindAll(c => c.Resultado == "derrota").Count,
                    Huidas = combates.FindAll(c => c.Resultado == "huida").Count,
                    TotalDanoInfligido = 0,
                    TotalDanoRecibido = 0,
                    TotalCriticos = 0,
                    ExpTotal = 0,
                    OroTotal = 0
                };

                foreach (var c in combates)
                {
                    stats.TotalDanoInfligido += c.DanoTotalJugador;
                    stats.TotalDanoRecibido += c.DanoTotalEnemigo;
                    stats.TotalCriticos += c.CriticosJugador;
                    stats.ExpTotal += c.ExpGanada;
                    stats.OroTotal += c.OroGanado;
                }

                if (stats.TotalCombates > 0)
                {
                    stats.PorcentajeVictoria = (float)stats.Victorias / stats.TotalCombates * 100f;
                }

                return stats;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error obteniendo estadísticas: {ex.Message}");
                return new EstadisticasCombate();
            }
        }

        /// <summary>
        /// Obtiene los combates de una sesión específica
        /// </summary>
        public List<CombatLog> ObtenerCombatesSesion(string sesionId)
        {
            try
            {
                return _collection.Find(c => c.SesionId == sesionId)
                                  .SortBy(c => c.Fecha)
                                  .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error obteniendo combates de sesión: {ex.Message}");
                return new List<CombatLog>();
            }
        }
    }

    /// <summary>
    /// Clase auxiliar para estadísticas de combate
    /// </summary>
    public class EstadisticasCombate
    {
        public int TotalCombates { get; set; }
        public int Victorias { get; set; }
        public int Derrotas { get; set; }
        public int Huidas { get; set; }
        public float PorcentajeVictoria { get; set; }
        public int TotalDanoInfligido { get; set; }
        public int TotalDanoRecibido { get; set; }
        public int TotalCriticos { get; set; }
        public int ExpTotal { get; set; }
        public int OroTotal { get; set; }
    }
}