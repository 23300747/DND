using MongoDB.Driver;
using MongoDB.Bson;
using System;

namespace Proyecto_Dnd.Database.MongoDB
{
    public class MongoDBConfig
    {
        private static IMongoDatabase _database;
        private static readonly string ConnectionString = "mongodb+srv://DNDMaster:Maestro@proyectointegrador.zq5wruc.mongodb.net/";
        private static readonly string DatabaseName = "rpg_game_logs";

        public static IMongoDatabase GetDatabase()
        {
            if (_database == null)
            {
                try
                {
                    var settings = MongoClientSettings.FromConnectionString(ConnectionString);
                    settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                    settings.ConnectTimeout = TimeSpan.FromSeconds(10);
                    settings.SocketTimeout = TimeSpan.FromSeconds(30);

                    var client = new MongoClient(settings);
                    _database = client.GetDatabase(DatabaseName);

                    // Verificar conexión - CORREGIDO
                    var command = new BsonDocument("ping", 1);
                    _database.RunCommand<BsonDocument>(command);

                    System.Diagnostics.Debug.WriteLine("✅ MongoDB Atlas conectado correctamente");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error conectando a MongoDB: {ex.Message}");
                    throw;
                }
            }
            return _database;
        }

        public static void CrearIndices()
        {
            try
            {
                var db = GetDatabase();

                // Índices para combat_logs
                var combatCollection = db.GetCollection<Models.CombatLog>("combat_logs");

                var combatIndexKeys1 = Builders<Models.CombatLog>.IndexKeys
                    .Ascending(c => c.JugadorId)
                    .Descending(c => c.Fecha);
                combatCollection.Indexes.CreateOne(new CreateIndexModel<Models.CombatLog>(combatIndexKeys1));

                var combatIndexKeys2 = Builders<Models.CombatLog>.IndexKeys
                    .Ascending(c => c.SesionId);
                combatCollection.Indexes.CreateOne(new CreateIndexModel<Models.CombatLog>(combatIndexKeys2));

                // Índices para game_sessions
                var sessionCollection = db.GetCollection<Models.GameSession>("game_sessions");

                var sessionIndexKeys1 = Builders<Models.GameSession>.IndexKeys
                    .Ascending(s => s.JugadorId)
                    .Descending(s => s.Inicio);
                sessionCollection.Indexes.CreateOne(new CreateIndexModel<Models.GameSession>(sessionIndexKeys1));

                var sessionIndexKeys2 = Builders<Models.GameSession>.IndexKeys
                    .Ascending(s => s.Activa);
                sessionCollection.Indexes.CreateOne(new CreateIndexModel<Models.GameSession>(sessionIndexKeys2));

                // Índices para events
                var eventCollection = db.GetCollection<Models.PlayerEvent>("events");

                var eventIndexKeys = Builders<Models.PlayerEvent>.IndexKeys
                    .Ascending(e => e.JugadorId)
                    .Descending(e => e.Timestamp);
                eventCollection.Indexes.CreateOne(new CreateIndexModel<Models.PlayerEvent>(eventIndexKeys));

                System.Diagnostics.Debug.WriteLine("✅ Índices de MongoDB creados");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Error creando índices: {ex.Message}");
            }
        }
    }
}