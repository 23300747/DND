using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Proyecto_Dnd.Database.MongoDB.Models
{
    public class PlayerEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("jugador_id")]
        public int JugadorId { get; set; }

        [BsonElement("session_id")]
        public string SessionId { get; set; }

        [BsonElement("tipo_evento")]
        public string TipoEvento { get; set; }
        // Tipos: "combate", "compra", "venta", "dialogo", "nivel_up", "muerte", "logro", "exploracion"

        [BsonElement("timestamp")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Timestamp { get; set; }

        [BsonElement("descripcion")]
        public string Descripcion { get; set; }

        [BsonElement("contexto")]
        public ContextoJugador Contexto { get; set; }

        [BsonElement("detalles")]
        public BsonDocument Detalles { get; set; }

        [BsonElement("importancia")]
        public string Importancia { get; set; } // "baja", "normal", "alta", "critica"

        public PlayerEvent()
        {
            Timestamp = DateTime.Now;
            Importancia = "normal";
        }
    }

    public class ContextoJugador
    {
        [BsonElement("nivel")]
        public int Nivel { get; set; }

        [BsonElement("hp_actual")]
        public int HPActual { get; set; }

        [BsonElement("hp_maximo")]
        public int HPMaximo { get; set; }

        [BsonElement("oro")]
        public int Oro { get; set; }

        [BsonElement("ubicacion")]
        public string Ubicacion { get; set; }
    }
}