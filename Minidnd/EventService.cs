using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Proyecto_Dnd.Database.MongoDB.Models;
using MongoDB.Bson;

namespace Proyecto_Dnd.Database.MongoDB.Services
{
    public class EventService
    {
        private readonly IMongoCollection<PlayerEvent> _collection;

        public EventService()
        {
            var db = MongoDBConfig.GetDatabase();
            _collection = db.GetCollection<PlayerEvent>("player_events");
        }

        /// <summary>
        /// Registra un evento del jugador
        /// </summary>
        public void RegistrarEvento(int jugadorId, string sessionId, string tipoEvento,
            string descripcion, ContextoJugador contexto, BsonDocument detalles = null,
            string importancia = "normal")
        {
            try
            {
                var evento = new PlayerEvent
                {
                    JugadorId = jugadorId,
                    SessionId = sessionId,
                    TipoEvento = tipoEvento,
                    Descripcion = descripcion,
                    Contexto = contexto,
                    Detalles = detalles ?? new BsonDocument(),
                    Importancia = importancia,
                    Timestamp = DateTime.Now
                };

                _collection.InsertOne(evento);
                System.Diagnostics.Debug.WriteLine($"✅ Evento registrado: {tipoEvento} - {descripcion}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error registrando evento: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra una compra
        /// </summary>
        public void RegistrarCompra(int jugadorId, string sessionId, string nombreItem,
            int precio, ContextoJugador contexto)
        {
            var detalles = new BsonDocument
            {
                { "item", nombreItem },
                { "precio", precio }
            };

            RegistrarEvento(jugadorId, sessionId, "compra",
                $"Compró {nombreItem} por {precio} oro", contexto, detalles);
        }

        /// <summary>
        /// Registra una subida de nivel
        /// </summary>
        public void RegistrarSubidaNivel(int jugadorId, string sessionId, int nivelAnterior,
            int nivelNuevo, ContextoJugador contexto)
        {
            var detalles = new BsonDocument
            {
                { "nivel_anterior", nivelAnterior },
                { "nivel_nuevo", nivelNuevo }
            };

            RegistrarEvento(jugadorId, sessionId, "nivel_up",
                $"¡Subió de nivel {nivelAnterior} a {nivelNuevo}!", contexto, detalles, "alta");
        }

        /// <summary>
        /// Registra una muerte
        /// </summary>
        public void RegistrarMuerte(int jugadorId, string sessionId, string causaMuerte,
            ContextoJugador contexto)
        {
            var detalles = new BsonDocument
            {
                { "causa", causaMuerte }
            };

            RegistrarEvento(jugadorId, sessionId, "muerte",
                $"Cayó en combate: {causaMuerte}", contexto, detalles, "alta");
        }

        /// <summary>
        /// Registra un diálogo con NPC
        /// </summary>
        public void RegistrarDialogo(int jugadorId, string sessionId, string npcNombre,
            ContextoJugador contexto)
        {
            var detalles = new BsonDocument
            {
                { "npc", npcNombre }
            };

            RegistrarEvento(jugadorId, sessionId, "dialogo",
                $"Habló con {npcNombre}", contexto, detalles, "baja");
        }

        /// <summary>
        /// Registra exploración de zona
        /// </summary>
        public void RegistrarExploracion(int jugadorId, string sessionId, string zona,
            ContextoJugador contexto)
        {
            var detalles = new BsonDocument
            {
                { "zona", zona }
            };

            RegistrarEvento(jugadorId, sessionId, "exploracion",
                $"Descubrió {zona}", contexto, detalles, "normal");
        }

        /// <summary>
        /// Registra desbloqueo de logro
        /// </summary>
        public void RegistrarLogro(int jugadorId, string sessionId, string nombreLogro,
            string rareza, ContextoJugador contexto)
        {
            var detalles = new BsonDocument
            {
                { "logro", nombreLogro },
                { "rareza", rareza }
            };

            RegistrarEvento(jugadorId, sessionId, "logro",
                $"¡Logro desbloqueado: {nombreLogro}!", contexto, detalles, "critica");
        }

        /// <summary>
        /// Obtiene eventos de un jugador
        /// </summary>
        public List<PlayerEvent> ObtenerEventosJugador(int jugadorId, int limite = 50)
        {
            try
            {
                return _collection.Find(e => e.JugadorId == jugadorId)
                                  .SortByDescending(e => e.Timestamp)
                                  .Limit(limite)
                                  .ToList();
            }
            catch
            {
                return new List<PlayerEvent>();
            }
        }

        /// <summary>
        /// Obtiene eventos por tipo
        /// </summary>
        public List<PlayerEvent> ObtenerEventosPorTipo(int jugadorId, string tipo)
        {
            try
            {
                return _collection.Find(e => e.JugadorId == jugadorId && e.TipoEvento == tipo)
                                  .SortByDescending(e => e.Timestamp)
                                  .ToList();
            }
            catch
            {
                return new List<PlayerEvent>();
            }
        }

        /// <summary>
        /// Obtiene eventos importantes
        /// </summary>
        public List<PlayerEvent> ObtenerEventosImportantes(int jugadorId)
        {
            try
            {
                return _collection.Find(e => e.JugadorId == jugadorId &&
                                            (e.Importancia == "alta" || e.Importancia == "critica"))
                                  .SortByDescending(e => e.Timestamp)
                                  .ToList();
            }
            catch
            {
                return new List<PlayerEvent>();
            }
        }
    }
}