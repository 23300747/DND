using System;
using System.Drawing;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Linq;
using static Proyecto_Dnd.Form10;
using Proyecto_Dnd.Database.MongoDB.Services;
using System.Media;
using System.Data;

namespace Proyecto_Dnd
{
    public partial class Form5 : Form
    {
        private Database.MongoDB.Services.EventService eventService;
        private Dictionary<string, (int y, int x)> retiroDragon = new Dictionary<string, (int y, int x)>
        {
            { "Runara",  (10, 13) },
            { "Laylee",  (8, 13) },
            { "Zark",    (13,13 ) },
            { "Tarak",   (13,8) },
            { "Mila",    (8, 8) },
            { "Biblioteca",    (14, 10) }
        };

        private int usuarioId = Form9.JugadorIdActual;
        private const int Tamano = 50;
        private const int filas = 25;
        private const int columnas = 40;
        private Tile[,] mapa = new Tile[filas, columnas];
        private Personaje jugador;
        private Panel MAP;
        private Label lblEstadoJugador;
        private Button btnInventario, btnSalir, btnLogros, btnEstadisticas, btnDescanso;
        private bool botonesVisibles = true;
        private SoundPlayer musicaFondo;
        private SoundPlayer sonidoBitacora;
        public Form5()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            eventService = new Database.MongoDB.Services.EventService();

            musicaFondo = new SoundPlayer("Musica.wav");
            musicaFondo.PlayLooping();
            sonidoBitacora = new SoundPlayer("bitacora.wav");

            MAP = new Panel();
            MAP.AutoScroll = true;
            MAP.Size = this.ClientSize;
            MAP.Location = new Point(0, 0);
            this.Controls.Add(MAP);

            Mapa();
            CrearPersonaje();


            btnInventario = new Button
            {
                Text = "INVENTARIO",
                Size = new Size(160, 40),
                Location = new Point(10, 10),
                BackColor = Color.SaddleBrown,
                ForeColor = Color.White,
                Font = new Font("Papyrus", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnInventario.FlatAppearance.BorderColor = Color.Gold;
            btnInventario.Click += (s, e2) =>
            {
                Form10 inventario = new Form10(usuarioId);
                inventario.ShowDialog();
                ActualizarEstadoUI();
            };
            this.Controls.Add(btnInventario);
            btnInventario.BringToFront();

            btnSalir = new Button
            {
                Text = "SALIR",
                Size = new Size(160, 40),
                Location = new Point(180, 10),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Papyrus", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnSalir.FlatAppearance.BorderColor = Color.Gold;
            btnSalir.Click += BtnSalir_Click;
            this.Controls.Add(btnSalir);
            btnSalir.BringToFront();
            btnLogros = new Button  // ← QUITAR "Button" AQUÍ
            {
                Text = "LOGROS",
                Size = new Size(160, 40),
                Location = new Point(350, 10),
                BackColor = Color.DarkGoldenrod,
                ForeColor = Color.White,
                Font = new Font("Papyrus", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnLogros.FlatAppearance.BorderColor = Color.Gold;
            btnLogros.Click += (s, e2) =>
            {
                Form12 logros = new Form12(usuarioId);
                logros.ShowDialog();
            };
            this.Controls.Add(btnLogros);
            btnLogros.BringToFront();

            btnEstadisticas = new Button  // ← QUITAR "Button" AQUÍ
            {
                Text = "ESTADÍSTICAS",
                Size = new Size(160, 40),
                Location = new Point(520, 10),
                BackColor = Color.DarkSlateBlue,
                ForeColor = Color.White,
                Font = new Font("Papyrus", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnEstadisticas.FlatAppearance.BorderColor = Color.Gold;
            btnEstadisticas.Click += (s, e2) =>
            {
                Form13 stats = new Form13(usuarioId);
                stats.ShowDialog();
            };
            this.Controls.Add(btnEstadisticas);
            btnEstadisticas.BringToFront();

            btnDescanso = new Button  // ← QUITAR "Button" AQUÍ
            {
                Text = "DESCANSAR",
                Size = new Size(160, 40),
                Location = new Point(690, 10),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                Font = new Font("Papyrus", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnDescanso.FlatAppearance.BorderColor = Color.Gold;
            btnDescanso.Click += (s, e2) =>
            {
                try
                {
                    using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                    {
                        conexion.Open();
                        using (MySqlCommand cmd = new MySqlCommand("ObtenerHPJugador", conexion))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@jugadorId", usuarioId);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int hpActual = reader.GetInt32("HPActual");
                                    int hpMax = reader.GetInt32("HPMax");

                                    Form15 descanso = new Form15(usuarioId, hpActual, hpMax);
                                    if (descanso.ShowDialog() == DialogResult.OK)
                                    {
                                        ActualizarEstadoUI();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            this.Controls.Add(btnDescanso);
            btnDescanso.BringToFront();

            lblEstadoJugador = new Label
            {
                Location = new Point(10, 60),
                Size = new Size(200, 80),
                BackColor = Color.FromArgb(60, 45, 30),
                ForeColor = Color.White,
                Font = new Font("Papyrus", 9, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };
            this.Controls.Add(lblEstadoJugador);
            lblEstadoJugador.BringToFront();
            ActualizarEstadoUI();
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
                "¿Deseas guardar tu progreso antes de salir?",
                "Confirmar salida",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                GuardarProgreso();
                Application.Exit();
            }
            else if (resultado == DialogResult.No)
            {
                Application.Exit();
            }
        }

        private void GuardarProgreso()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();
                    using (MySqlCommand cmd = new MySqlCommand("GuardarProgresoJugador", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("pJugadorId", usuarioId);
                        cmd.Parameters.AddWithValue("pHP", jugador.HP);
                        cmd.Parameters.AddWithValue("pEXP", jugador.EXP);
                        cmd.Parameters.AddWithValue("pOro", jugador.Oro);

                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show($"✓ Progreso guardado\n\n❤ HP: {jugador.HP}\n⭐ EXP: {jugador.EXP}\n💰 Oro: {jugador.Oro}",
                                "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Mapa()
        {
            this.BackgroundImage = Image.FromFile("map1.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Text = "Dungeons && Dragons";

            for (int y = 0; y < filas; y++)
            {
                for (int x = 0; x < columnas; x++)
                {
                    Panel tilePanel = new Panel
                    {
                        Size = new Size(Tamano, Tamano),
                        Location = new Point(x * Tamano, y * Tamano),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackgroundImage = CargarImagenDesdeRecursos("pasto.png"),
                        BackgroundImageLayout = ImageLayout.Stretch
                    };

                    MAP.Controls.Add(tilePanel);

                    mapa[y, x] = new Tile
                    {
                        X = x,
                        Y = y,
                        PanelVisual = tilePanel,
                        EsZona = false,
                        EsObstaculo = false,
                        Nombre = $"Tile {x},{y}",
                        IDEnemigo = 0
                    };
                }
            }

            // Arena
            for (int y = 0; y <= 4; y++)
            {
                for (int x = 0; x <= 4; x++)
                {
                    SetFondo(mapa[y, x].PanelVisual, "arena.png");
                }
            }

            // Entradas
            SetEntrada(0, 0, "entrada3.png");
            SetEntrada(12, 39, "entrada2.png");
            SetEntrada(24, 0, "entrada1.png");

            // NPCs del Retiro
            foreach (var kvp in retiroDragon)
            {
                var nombre = kvp.Key;
                var (y, x) = kvp.Value;
                var panel = mapa[y, x].PanelVisual;
                panel.BackgroundImage = CargarImagenDesdeRecursos("NPC.png");
                panel.BringToFront();
                mapa[y, x].EsZona = true;
                mapa[y, x].EsObstaculo = nombre != "Biblioteca";
                mapa[y, x].Nombre = nombre == "Biblioteca" ? "Biblioteca del Retiro" : $"Retiro del Dragón – {nombre}";
            }

            // Muros
            int[][] muros = new int[][]
            {
                new int[] {7,7}, new int[] {7,8}, new int[] {7,9}, new int[] {7,11}, new int[] {7,12},
                new int[] {7,13}, new int[] {7,14}, new int[] {14,7}, new int[] {14,8}, new int[] {14,9},
                new int[] {14,11}, new int[] {14,12}, new int[] {14,13}, new int[] {14,14}, new int[] {8,7},
                new int[] {9,7}, new int[] {11,7}, new int[] {12,7}, new int[] {13,7}, new int[] {14,7},
                new int[] {7,14}, new int[] {8,14}, new int[] {9,14}, new int[] {10,14}, new int[] {11,14},
                new int[] {12,14}, new int[] {13,14}, new int[] {14,14}
            };
            foreach (var pos in muros) { SetObstaculo(pos[0], pos[1]); }

            SetBiblioteca(14, 10, "biblioteca del retiro", "biblio.png");

            // Montañas
            Setmontana(5, 0); Setmontana(5, 1); Setmontana(5, 2); Setmontana(5, 3);
            Setmontana(0, 5); Setmontana(1, 5); Setmontana(2, 5); Setmontana(3, 5);

            // Enemigos con IDs: Zombi (1), Ahogado (2), Pulpo fúngico (3)
            SetEnemigo(4, 4, "Zombi", "zombi.png", 1);
            SetEnemigo(8, 6, "Zombi", "zombi.png", 1);
            SetEnemigo(12, 6, "Zombi", "zombi.png", 1);
            SetEnemigo(13, 12, "Zombi", "zombi.png", 1);
            SetEnemigo(10, 6, "Zombi", "zombi.png", 1);
            SetEnemigo(0, 15, "Ahogado", "zombi.png", 2);
            SetEnemigo(0, 17, "Ahogado", "zombi.png", 2);
            SetEnemigo(2, 16, "Ahogado", "zombi.png", 2);
            SetEnemigo(20, 20, "Pulpo fúngico", "pulpo.png", 3);
            SetEnemigo(3, 18, "Zombi", "zombi.png", 1);
            SetEnemigo(6, 25, "Zombi", "zombi.png", 1);
            SetEnemigo(9, 30, "Zombi", "zombi.png", 1);
            SetEnemigo(15, 5, "Ahogado", "zombi.png", 2);
            SetEnemigo(17, 12, "Ahogado", "zombi.png", 2);
            SetEnemigo(22, 35, "Ahogado", "zombi.png", 2);
            SetEnemigo(24, 10, "Zombi", "zombi.png", 1);
            SetEnemigo(19, 28, "Zombi", "zombi.png", 1);
            SetEnemigo(5, 23, "Zombi", "zombi.png", 1);

            // Zonas especiales
            mapa[21, 5].EsZona = true;
            mapa[21, 5].Nombre = "Aguas termales";
            mapa[18, 18].EsZona = true;
            mapa[18, 18].Nombre = "Territorio del oso lechuza";
            mapa[22, 10].EsZona = true;
            mapa[22, 10].Nombre = "Emboscada de kobolds";
        }

        private void SetEntrada(int y, int x, string imageName)
        {
            mapa[y, x].EsZona = true;
            mapa[y, x].EsObstaculo = false;
            SetFondo(mapa[y, x].PanelVisual, imageName);
        }

        private void SetObstaculo(int y, int x)
        {
            mapa[y, x].EsObstaculo = true;
            mapa[y, x].Nombre = "Muro bloqueado";
            SetFondo(mapa[y, x].PanelVisual, "casa.png");
        }

        private void Setmontana(int y, int x)
        {
            mapa[y, x].EsObstaculo = true;
            mapa[y, x].Nombre = "Muro bloqueado";
            SetFondo(mapa[y, x].PanelVisual, "montana.png");
        }

        private void SetEnemigo(int y, int x, string tipo, string imageName, int idEnemigoDB)
        {
            var panel = mapa[y, x].PanelVisual;
            SetFondo(panel, imageName);
            panel.BringToFront();
            mapa[y, x].EsZona = true;
            mapa[y, x].Nombre = $"Enemigo: {tipo}";
            mapa[y, x].IDEnemigo = idEnemigoDB;
        }

        private void SetBiblioteca(int y, int x, string nombre, string imageName)
        {
            mapa[y, x].EsZona = true;
            mapa[y, x].EsObstaculo = false;
            mapa[y, x].Nombre = nombre;
            SetFondo(mapa[y, x].PanelVisual, imageName);
        }

        private void CrearPersonaje()
        {
            jugador = new Personaje { X = 2, Y = 2 };

            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();
                    using (MySqlCommand cmd = new MySqlCommand("ObtenerPersonaje", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("pJugadorId", usuarioId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                jugador.HP = reader.GetInt32("HP");
                                jugador.EXP = reader.GetInt32("EXP");
                                jugador.Oro = reader.GetInt32("Oro");
                                jugador.ClaseId = reader.GetInt32("ID_Clase");
                                jugador.Clase = reader.GetString("Clase");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear personaje:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        

        Panel visual = new Panel
            {
                Size = new Size(Tamano, Tamano),
                Location = mapa[jugador.Y, jugador.X].PanelVisual.Location,
                BackColor = Color.Transparent
            };

            string rutaImagen = jugador.ClaseId switch
            {
                1 => "guerrero.png",
                2 => "mago.png",
                3 => "paladin.png",
                4 => "clerigo.png",
                _ => "guerrero.png"
            };

            if (System.IO.File.Exists(rutaImagen))
            {
                visual.BackgroundImage = Image.FromFile(rutaImagen);
                visual.BackgroundImageLayout = ImageLayout.Stretch;
            }

            jugador.Visual = visual;
            MAP.Controls.Add(visual);
            jugador.Visual.BringToFront();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                botonesVisibles = !botonesVisibles;
                btnInventario.Visible = botonesVisibles;
                btnSalir.Visible = botonesVisibles;
                btnLogros.Visible = botonesVisibles;          
                btnEstadisticas.Visible = botonesVisibles;     
                btnDescanso.Visible = botonesVisibles;
                lblEstadoJugador.Visible = botonesVisibles;
                return true;
            }

            int nuevoX = jugador.X;
            int nuevoY = jugador.Y;
            switch (keyData)
            {
                case Keys.W: nuevoY--; break;
                case Keys.S: nuevoY++; break;
                case Keys.A: nuevoX--; break;
                case Keys.D: nuevoX++; break;
            }

            if (nuevoX >= 0 && nuevoX < columnas && nuevoY >= 0 && nuevoY < filas)
            {
                Tile destino = mapa[nuevoY, nuevoX];
                if (!destino.EsObstaculo)
                {
                    jugador.X = nuevoX;
                    jugador.Y = nuevoY;
                    jugador.Visual.Location = destino.PanelVisual.Location;
                    jugador.Visual.BringToFront();
                    MAP.ScrollControlIntoView(jugador.Visual);

                    // Transiciones
                    if (jugador.X == 0 && jugador.Y == filas - 1)
                    {
                        MessageBox.Show("Una oscuridad sombría te rodea y te sientes cansado. Las cuevas marinas te esperan...", "Entrada a las cuevas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide();
                        RegistrarCambioZona("Cuevas Marinas");
                        new Form6().Show();
                        return true;
                    }
                    if (jugador.X == 0 && jugador.Y == 0)
                    {
                        MessageBox.Show("Los restos del barco yacen ante ti. Una voz susurra: 'Sube al Rosa de los Vientos...'", "Pecio del barco", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.Hide();
                        RegistrarCambioZona("Pecio");
                        new Form7().Show();
                        return true;
                    }
                    if (jugador.X == 39 && jugador.Y == 12)
                    {
                        MessageBox.Show("Escuchas los ecos de antiguos guerreros. El observatorio te llama...", "Entrada al observatorio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide();
                        RegistrarCambioZona("Observatorio");
                        new Form8().Show();
                        return true;
                    }
                }
            }

            // Evento especial de Runara
            if (jugador.X == 13 && jugador.Y == 10)
            {
                MessageBox.Show("Runara sonríe. \"Tengo algo que mostraros...\"", "Encuentro narrativo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("Un destello... aparece un dragón de escamas bronce. \"Ahora me veis con mi apariencia real.\"", "Runara revelada", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                MessageBox.Show("Runara les entrega una llave de piedra lunar para entrar al observatorio.", "Objeto clave obtenido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            var tileActual = mapa[jugador.Y, jugador.X];

            // SISTEMA DE COMBATE
            if (tileActual.Nombre.StartsWith("Enemigo:") && tileActual.IDEnemigo > 0)
            {
                this.Hide();
                Form16 combate = new Form16(tileActual.IDEnemigo, this);
                DialogResult resultado = combate.ShowDialog();

                // Solo eliminar enemigo si ganó (DialogResult.OK)
                if (resultado == DialogResult.OK)
                {
                    SetFondo(tileActual.PanelVisual, "pasto.png");
                    tileActual.Nombre = $"Tile {jugador.X},{jugador.Y}";
                    tileActual.EsZona = false;
                    tileActual.IDEnemigo = 0;
                    MessageBox.Show("¡El enemigo ha sido derrotado y desaparece del mapa!", "Victoria", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (resultado == DialogResult.Cancel)
                {
                    // Derrota - Enemigo sigue ahí
                    MessageBox.Show("Has sido derrotado. El enemigo permanece en su posición.", "Derrota", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (resultado == DialogResult.Abort)
                {
                    // Huida - Enemigo sigue ahí
                    MessageBox.Show("Lograste huir. El enemigo permanece vigilante.", "Huida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ActualizarEstadoUI();
                this.Show();
            }

            // Interacción con NPCs (tecla E)
            if (keyData == Keys.E)
            {
                foreach (var kvp in retiroDragon)
                {
                    var (py, px) = kvp.Value;
                    if (Math.Abs(jugador.X - px) + Math.Abs(jugador.Y - py) == 1)
                    {
                        string nombre = kvp.Key;

                        if (nombre == "Runara")
                        {
                            try
                            {
                                sonidoBitacora?.Play(); // Reproducir sonido 1 vez
                            }
                            catch { }

                            MessageBox.Show("Runara: 'Bienvenido al Retiro del Dragón, viajero. Aquí encontrarás paz y sabiduría. Encontre uan bitacora para que la escuches'", "Runara la Anciana", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (nombre == "Tarak")
                        {
                            MessageBox.Show("Tarak: 'Necesito hongos con sombrero de corazón de las cuevas. ¿Puedes ayudarme? A cambio te prepararé pociones.'", "Tarak el Herbolario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (nombre == "Laylee")
                        {
                            MessageBox.Show("Laylee: 'Fui general de los Lobos Celestes. Ahora busco redención en este lugar. Vi el barco estrellarse contra las rocas... algo no está bien ahí.'", "Laylee la Veterana", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (nombre == "Mila")
                        {
                            MostrarCuadroMisiones();
                        }
                        else if (nombre == "Zark")
                        {
                            MessageBox.Show("Zark: 'Bienvenido a mi tienda. Tengo los mejores suministros de toda la isla.'", "Zark el Comerciante", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Form11 tienda = new Form11(usuarioId);
                            tienda.ShowDialog();
                            ActualizarEstadoUI();
                        }
                        else if (nombre == "Biblioteca")
                        {
                            MessageBox.Show("Entras a la biblioteca. Libros antiguos llenan los estantes, algunos hablan de dragones y leyendas olvidadas.", "Biblioteca del Retiro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void RegistrarCambioZona(string nombreZona)
        {
            try
            {
                int hpMaximo = 100; // valor por defecto
                int nivel = 1;      // valor por defecto

                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();
                    using (MySqlCommand cmd = new MySqlCommand("ObtenerHPMaxYNivel", conexion))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("pJugadorId", usuarioId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hpMaximo = reader.IsDBNull(0) ? 100 : reader.GetInt32("HPMax");
                                nivel = reader.GetInt32("Nivel");
                            }
                        }
                    }
                }

                // 🔥 Aquí no tocamos nada del código Mongo
                var contexto = new Database.MongoDB.Models.ContextoJugador
                {
                    Nivel = nivel,
                    HPActual = jugador.HP,
                    HPMaximo = hpMaximo,
                    Oro = jugador.Oro,
                    Ubicacion = nombreZona,
                };

                eventService.RegistrarExploracion(
                    usuarioId,
                    Form9.SesionIdActual,
                    nombreZona,
                    contexto
                );

                var sessionService = new Database.MongoDB.Services.SessionService();
                sessionService.AgregarZonaVisitada(Form9.SesionIdActual, nombreZona);

                System.Diagnostics.Debug.WriteLine($"✅ Cambio de zona registrado: {nombreZona}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Error registrando cambio de zona: {ex.Message}");
            }
        }

        private void MostrarCuadroMisiones()
        {
            Form cuadro = new Form
            {
                Text = "Misiones disponibles - Mila la Kobold",
                Size = new Size(400, 320),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var misiones = new[]
            {
                ("Cuevas Marinas", "Matar al pulpo fúngico"),
                ("Pecio del Rosa de los Vientos", "Enfrentar zombis marinos"),
                ("Reaparición de zombis", "Defender el Retiro del Dragón"),
                ("Este reino está en tus manos", "Explora la isla y protege su destino")
            };

            int y = 20;
            foreach (var (titulo, objetivo) in misiones)
            {
                Label lbl = new Label
                {
                    Text = $"{titulo}\n→ {objetivo}",
                    Location = new Point(20, y),
                    Size = new Size(340, 40),
                    Font = new Font("Segoe UI", 10)
                };
                cuadro.Controls.Add(lbl);
                y += 50;
            }

            Button cerrar = new Button
            {
                Text = "Cerrar",
                Size = new Size(100, 30),
                Location = new Point(140, y)
            };
            cerrar.Click += (s, e) => cuadro.Close();
            cuadro.Controls.Add(cerrar);
            cuadro.ShowDialog();
        }

        private void SetFondo(Panel panel, string nombreImagen)
        {
            try
            {
                Image fondoBase = CargarImagenDesdeRecursos("pasto.png");
                Image imagen = CargarImagenDesdeRecursos(nombreImagen);

                if (fondoBase != null && imagen != null)
                {
                    Bitmap combinado = new Bitmap(Tamano, Tamano);
                    using (Graphics g = Graphics.FromImage(combinado))
                    {
                        g.DrawImage(fondoBase, 0, 0, Tamano, Tamano);
                        g.DrawImage(imagen, 0, 0, Tamano, Tamano);
                    }
                    panel.BackgroundImage = combinado;
                    panel.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
            catch { }
        }

        public void MoverJugadorA(int x, int y)
        {
            if (jugador != null && x >= 0 && x < columnas && y >= 0 && y < filas)
            {
                jugador.X = x;
                jugador.Y = y;
                jugador.Visual.Location = mapa[y, x].PanelVisual.Location;
                jugador.Visual.BringToFront();
                MAP.ScrollControlIntoView(jugador.Visual);
            }
        }

        public class Tile
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Panel PanelVisual { get; set; }
            public bool EsZona { get; set; }
            public bool EsObstaculo { get; set; }
            public string Nombre { get; set; }
            public int IDEnemigo { get; set; }
        }

        public class Personaje
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Panel Visual { get; set; }
            public int HP { get; set; } = 100;
            public int EXP { get; set; } = 0;
            public int Oro { get; set; } = 0;
            public int ClaseId { get; set; }
            public string Clase { get; set; }
        }


        private Image CargarImagenDesdeRecursos(string nombreImagen)
        {
            var ensamblado = Assembly.GetExecutingAssembly();
            var rutaCompleta = $"Proyecto_Dnd.Recursos.{nombreImagen}";

            using (Stream stream = ensamblado.GetManifestResourceStream(rutaCompleta))
            {
                if (stream != null)
                    return Image.FromStream(stream);
            }
            return null;
        }

        private void ActualizarEstadoUI()
        {
            if (lblEstadoJugador != null)
            {
                try
                {
                    using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                    {
                        conexion.Open();
                        using (MySqlCommand cmd = new MySqlCommand("ObtenerEstadoJugador", conexion))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("pJugadorId", usuarioId);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    jugador.HP = reader.GetInt32("HP");
                                    jugador.EXP = reader.GetInt32("EXP");
                                    jugador.Oro = reader.GetInt32("Oro");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Error al actualizar estado: {ex.Message}");
                }

                lblEstadoJugador.Text = $"❤ HP: {jugador.HP}\n⭐ EXP: {jugador.EXP}\n💰 Oro: {jugador.Oro}";
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            musicaFondo?.Stop();
            musicaFondo?.Dispose();
            sonidoBitacora?.Dispose();
            base.OnFormClosing(e);
        }
    }
}