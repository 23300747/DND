using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Proyecto_Dnd.Database.MongoDB.Models
{
    public class GameAnalytics
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("fecha")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Fecha { get; set; }

        [BsonElement("tipo")]
        public string Tipo { get; set; } // "diario", "semanal", "mensual"

        [BsonElement("jugadores_activos")]
        public int JugadoresActivos { get; set; }

        [BsonElement("combates_totales")]
        public int CombatesTotales { get; set; }

        [BsonElement("tasa_victoria")]
        public double TasaVictoria { get; set; }

        [BsonElement("enemigos_derrotados")]
        public Dictionary<string, int> EnemigosLerrotados { get; set; }

        [BsonElement("clases_mas_jugadas")]
        public Dictionary<string, int> ClasesMasJugadas { get; set; }

        [BsonElement("oro_total_economia")]
        public long OroTotalEconomia { get; set; }

        [BsonElement("nivel_promedio")]
        public double NivelPromedio { get; set; }

        public GameAnalytics()
        {
            Fecha = DateTime.Now;
            EnemigosLerrotados = new Dictionary<string, int>();
            ClasesMasJugadas = new Dictionary<string, int>();
        }
    }
}