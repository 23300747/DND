using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Proyecto_Dnd.Database.MongoDB.Models
{
    /// <summary>
    /// Modelo para representar una sesión de juego completa
    /// </summary>
    public class GameSession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("jugador_id")]
        public int JugadorId { get; set; }

        [BsonElement("jugador_nombre")]
        public string JugadorNombre { get; set; }

        [BsonElement("inicio")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Inicio { get; set; }

        [BsonElement("fin")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? Fin { get; set; }

        [BsonElement("duracion_minutos")]
        public int DuracionMinutos { get; set; }

        [BsonElement("activa")]
        public bool Activa { get; set; }

        [BsonElement("personaje_inicio")]
        public PersonajeEstado PersonajeInicio { get; set; }

        [BsonElement("personaje_fin")]
        public PersonajeEstado PersonajeFin { get; set; }

        [BsonElement("combates_ganados")]
        public int CombatesGanados { get; set; }

        [BsonElement("combates_perdidos")]
        public int CombatesPerdidos { get; set; }

        [BsonElement("oro_ganado")]
        public int OroGanado { get; set; }

        [BsonElement("oro_gastado")]
        public int OroGastado { get; set; }

        [BsonElement("exp_ganada")]
        public int ExpGanada { get; set; }

        [BsonElement("zonas_visitadas")]
        public List<string> ZonasVisitadas { get; set; }

        [BsonElement("eventos")]
        public List<EventoSesion> Eventos { get; set; }

        public GameSession()
        {
            Inicio = DateTime.Now;
            Activa = true;
            CombatesGanados = 0;
            CombatesPerdidos = 0;
            OroGanado = 0;
            OroGastado = 0;
            ExpGanada = 0;
            ZonasVisitadas = new List<string>();
            Eventos = new List<EventoSesion>();
        }
    }

    /// <summary>
    /// Estado del personaje en un momento específico
    /// </summary>
    public class PersonajeEstado
    {
        [BsonElement("nivel")]
        public int Nivel { get; set; }

        [BsonElement("hp")]
        public int HP { get; set; }

        [BsonElement("exp")]
        public int EXP { get; set; }

        [BsonElement("oro")]
        public int Oro { get; set; }

        [BsonElement("clase")]
        public string Clase { get; set; }

        [BsonElement("ubicacion")]
        public string Ubicacion { get; set; }
    }

    /// <summary>
    /// Evento ocurrido durante la sesión
    /// </summary>
    public class EventoSesion
    {
        [BsonElement("timestamp")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Timestamp { get; set; }

        [BsonElement("tipo")]
        public string Tipo { get; set; } // "combate", "exploracion", "comercio", "nivel_subido", etc.

        [BsonElement("descripcion")]
        public string Descripcion { get; set; }

        [BsonElement("datos")]
        public BsonDocument Datos { get; set; }

        public EventoSesion()
        {
            Timestamp = DateTime.Now;
            Datos = new BsonDocument();
        }
    }
}