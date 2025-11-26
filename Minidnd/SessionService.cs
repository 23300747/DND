using MongoDB.Driver;
using MongoDB.Bson; // ← IMPORTANTE: Este using
using System;
using System.Collections.Generic;
using Proyecto_Dnd.Database.MongoDB.Models;

namespace Proyecto_Dnd.Database.MongoDB.Services
{
    public class SessionService
    {
        private readonly IMongoCollection<GameSession> _collection;

        public SessionService()
        {
            var db = MongoDBConfig.GetDatabase();
            _collection = db.GetCollection<GameSession>("game_sessions");
        }

        /// <summary>
        /// Inicia una nueva sesión de juego
        /// </summary>
        public string IniciarSesion(int jugadorId, string nombreJugador, int nivel, int hp, int exp, int oro, string clase, string ubicacionInicial)
        {
            try
            {
                var sesion = new GameSession
                {
                    JugadorId = jugadorId,
                    JugadorNombre = nombreJugador,
                    Inicio = DateTime.Now,
                    Activa = true,
                    PersonajeInicio = new PersonajeEstado
                    {
                        Nivel = nivel,
                        HP = hp,
                        EXP = exp,
                        Oro = oro,
                        Clase = clase,
                        Ubicacion = ubicacionInicial
                    }
                };

                _collection.InsertOne(sesion);
                System.Diagnostics.Debug.WriteLine($"✅ Sesión iniciada: {nombreJugador} (ID: {sesion.Id})");
                return sesion.Id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error iniciando sesión: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Finaliza la sesión activa (MÉTODO AGREGADO)
        /// </summary>
        public void FinalizarSesion(string sesionId)
        {
            try
            {
                var filter = Builders<GameSession>.Filter.Eq(s => s.Id, sesionId);
                var sesion = _collection.Find(filter).FirstOrDefault();

                if (sesion != null && sesion.Activa)
                {
                    var duracion = (int)(DateTime.Now - sesion.Inicio).TotalMinutes;

                    var update = Builders<GameSession>.Update
                        .Set(s => s.Fin, DateTime.Now)
                        .Set(s => s.DuracionMinutos, duracion)
                        .Set(s => s.Activa, false);

                    _collection.UpdateOne(filter, update);
                    System.Diagnostics.Debug.WriteLine($"✅ Sesión finalizada: {sesion.JugadorNombre} - Duración: {duracion} min");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error finalizando sesión: {ex.Message}");
            }
        }

        /// <summary>
        /// Cierra la sesión activa de un jugador con estado final del personaje
        /// </summary>
        public void CerrarSesion(string sesionId, int nivelFin, int hpFin, int expFin, int oroFin, string ubicacionFin)
        {
            try
            {
                var filter = Builders<GameSession>.Filter.Eq(s => s.Id, sesionId);
                var sesion = _collection.Find(filter).FirstOrDefault();

                if (sesion != null)
                {
                    var duracion = (int)(DateTime.Now - sesion.Inicio).TotalMinutes;

                    var update = Builders<GameSession>.Update
                        .Set(s => s.Fin, DateTime.Now)
                        .Set(s => s.DuracionMinutos, duracion)
                        .Set(s => s.Activa, false)
                        .Set(s => s.PersonajeFin, new PersonajeEstado
                        {
                            Nivel = nivelFin,
                            HP = hpFin,
                            EXP = expFin,
                            Oro = oroFin,
                            Ubicacion = ubicacionFin
                        });

                    _collection.UpdateOne(filter, update);
                    System.Diagnostics.Debug.WriteLine($"✅ Sesión cerrada: {sesion.JugadorNombre}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error cerrando sesión: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra un combate en la sesión actual
        /// </summary>
        public void RegistrarCombate(string sesionId, bool victoria)
        {
            try
            {
                var filter = Builders<GameSession>.Filter.Eq(s => s.Id, sesionId);
                UpdateDefinition<GameSession> update;

                if (victoria)
                    update = Builders<GameSession>.Update.Inc(s => s.CombatesGanados, 1);
                else
                    update = Builders<GameSession>.Update.Inc(s => s.CombatesPerdidos, 1);

                _collection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error registrando combate en sesión: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra oro ganado o gastado
        /// </summary>
        public void RegistrarTransaccionOro(string sesionId, int cantidad, bool esGanancia)
        {
            try
            {
                var filter = Builders<GameSession>.Filter.Eq(s => s.Id, sesionId);
                UpdateDefinition<GameSession> update;

                if (esGanancia)
                    update = Builders<GameSession>.Update.Inc(s => s.OroGanado, cantidad);
                else
                    update = Builders<GameSession>.Update.Inc(s => s.OroGastado, cantidad);

                _collection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error registrando transacción: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra experiencia ganada
        /// </summary>
        public void RegistrarExperiencia(string sesionId, int exp)
        {
            try
            {
                var filter = Builders<GameSession>.Filter.Eq(s => s.Id, sesionId);
                var update = Builders<GameSession>.Update.Inc(s => s.ExpGanada, exp);
                _collection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error registrando EXP: {ex.Message}");
            }
        }

        /// <summary>
        /// Agrega una zona visitada
        /// </summary>
        public void AgregarZonaVisitada(string sesionId, string zona)
        {
            try
            {
                var filter = Builders<GameSession>.Filter.Eq(s => s.Id, sesionId);
                var update = Builders<GameSession>.Update.AddToSet(s => s.ZonasVisitadas, zona);
                _collection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error agregando zona: {ex.Message}");
            }
        }

        /// <summary>
        /// Agrega un evento a la sesión
        /// </summary>
        public void AgregarEvento(string sesionId, string tipo, string descripcion, BsonDocument datos = null)
        {
            try
            {
                var evento = new EventoSesion
                {
                    Timestamp = DateTime.Now,
                    Tipo = tipo,
                    Descripcion = descripcion,
                    Datos = datos ?? new BsonDocument()
                };

                var filter = Builders<GameSession>.Filter.Eq(s => s.Id, sesionId);
                var update = Builders<GameSession>.Update.Push(s => s.Eventos, evento);
                _collection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error agregando evento: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las últimas sesiones de un jugador
        /// </summary>
        public List<GameSession> ObtenerSesionesJugador(int jugadorId, int limite = 10)
        {
            try
            {
                return _collection.Find(s => s.JugadorId == jugadorId)
                                  .SortByDescending(s => s.Inicio)
                                  .Limit(limite)
                                  .ToList();
            }
            catch
            {
                return new List<GameSession>();
            }
        }

        /// <summary>
        /// Obtiene la sesión activa de un jugador
        /// </summary>
        public GameSession ObtenerSesionActiva(int jugadorId)
        {
            try
            {
                return _collection.Find(s => s.JugadorId == jugadorId && s.Activa)
                                  .SortByDescending(s => s.Inicio)
                                  .FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
    }
}