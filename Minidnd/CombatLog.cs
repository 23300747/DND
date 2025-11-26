using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Proyecto_Dnd.Database.MongoDB.Models
{
    /// <summary>
    /// Modelo para registrar cada combate individual
    /// </summary>
    public class CombatLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("jugador_id")]
        public int JugadorId { get; set; }

        [BsonElement("jugador_nombre")]
        public string JugadorNombre { get; set; }

        [BsonElement("sesion_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SesionId { get; set; }

        [BsonElement("enemigo_id")]
        public int EnemigoId { get; set; }

        [BsonElement("enemigo_nombre")]
        public string EnemigoNombre { get; set; }

        [BsonElement("nivel_jugador")]
        public int NivelJugador { get; set; }

        [BsonElement("nivel_enemigo")]
        public int NivelEnemigo { get; set; }

        [BsonElement("resultado")]
        public string Resultado { get; set; } // "victoria", "derrota", "huida"

        [BsonElement("duracion_turnos")]
        public int DuracionTurnos { get; set; }

        [BsonElement("hp_inicial_jugador")]
        public int HPInicialJugador { get; set; }

        [BsonElement("hp_final_jugador")]
        public int HPFinalJugador { get; set; }

        [BsonElement("hp_inicial_enemigo")]
        public int HPInicialEnemigo { get; set; }

        [BsonElement("hp_final_enemigo")]
        public int HPFinalEnemigo { get; set; }

        [BsonElement("dano_total_jugador")]
        public int DanoTotalJugador { get; set; }

        [BsonElement("dano_total_enemigo")]
        public int DanoTotalEnemigo { get; set; }

        [BsonElement("criticos_jugador")]
        public int CriticosJugador { get; set; }

        [BsonElement("exp_ganada")]
        public int ExpGanada { get; set; }

        [BsonElement("oro_ganado")]
        public int OroGanado { get; set; }

        [BsonElement("subio_nivel")]
        public bool SubioNivel { get; set; }

        [BsonElement("ubicacion")]
        public string Ubicacion { get; set; }

        [BsonElement("fecha")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Fecha { get; set; }

        public CombatLog()
        {
            Fecha = DateTime.Now;
        }
    }
}