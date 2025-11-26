using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static Proyecto_Dnd.Form10;
using System.Reflection;
using System.Linq;
using System.Media;
using System.Data;

namespace Proyecto_Dnd
{
    public partial class Form7 : Form
    {
        private Panel mapaPecio;
        private const int Tamano = 50;
        private const int filas = 12;
        private const int columnas = 20;
        private Tile[,] mapa = new Tile[filas, columnas];
        private Personaje jugador;
        private int usuarioId = Form9.JugadorIdActual;
        private Label lblEstadoJugador;
        private Button btnInventario, btnSalir, btnLogros, btnEstadisticas, btnDescanso;
        private bool botonesVisibles = true;
        private SoundPlayer musicaFondo;

        public Form7()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            musicaFondo = new SoundPlayer("Musica.wav");
            musicaFondo.PlayLooping();
            mapaPecio = new Panel
            {
                AutoScroll = true,
                Size = this.ClientSize,
                Location = new Point(0, 0)
            };
            this.Controls.Add(mapaPecio);
            Mapa();
            CrearPersonaje();
            MarcarObstaculosEjemplo(jugador.X, jugador.Y);

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
            this.BackgroundImage = Image.FromFile("map3.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Text = "Dungeons && Dragons - Pecio del Rosa de los Vientos";

            for (int y = 0; y < filas; y++)
            {
                for (int x = 0; x < columnas; x++)
                {
                    Panel panel = new Panel
                    {
                        Size = new Size(Tamano, Tamano),
                        Location = new Point(x * Tamano, y * Tamano),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackgroundImage = CargarImagenDesdeRecursos("agua.png"),
                        BackgroundImageLayout = ImageLayout.Stretch
                    };

                    mapaPecio.Controls.Add(panel);

                    mapa[y, x] = new Tile
                    {
                        X = x,
                        Y = y,
                        PanelVisual = panel,
                        EsZona = false,
                        EsObstaculo = false,
                        Nombre = $"Pecio {x},{y}",
                        IDEnemigo = 0
                    };
                }
            }
            SetFondo(mapa[9, 0].PanelVisual, "entrada3.png");

            SetZona(0, 10, "Zona C1 – Cubierta Superior");
            SetZona(1, 5, "Zona C2 – Castillo de Proa");
            SetZona(2, 15, "Zona C3 – Puesto de Mando");
            SetZona(3, 10, "Zona C4 – Camarote del Capitán");
            SetZona(4, 6, "Zona C5 – Cocina");
            SetZona(4, 14, "Zona C6 – Camarotes de la Tripulación");
            SetZona(5, 17, "Zona C7 – Comedor");
            SetZona(7, 10, "Zona C8 – Cubierta Inferior");
            SetZona(9, 10, "Zona C9 – Bodega");
            SetZona(10, 11, "Zona C10 – Cámara del Astrolabio");

            // Enemigos con IDs de la base de datos: Estirge (ID 18), Hongo violeta (ID 12), Draco de humo (ID 20)
            SetEnemigo(2, 9, "Estirge", "bicho.png", 18);
            SetEnemigo(4, 6, "Hongo violeta", "hongoB.png", 12);
            SetEnemigo(6, 12, "Hongo violeta", "hongoB.png", 12);
            SetEnemigo(8, 11, "Estirge", "bicho.png", 18);
            SetEnemigo(10, 18, "Draco de humo", "dragonhumo.png", 20);
        }

        private void SetZona(int y, int x, string nombre)
        {
            if (y < 0 || y >= filas || x < 0 || x >= columnas) return;

            var panel = mapa[y, x].PanelVisual;
            SetFondo(panel, "madera.png");
            mapa[y, x].EsZona = true;
            mapa[y, x].Nombre = nombre;
        }

        private void SetEnemigo(int y, int x, string tipo, string imageName, int idEnemigoDB)
        {
            if (y < 0 || y >= filas || x < 0 || x >= columnas) return;

            var panel = mapa[y, x].PanelVisual;
            SetFondo(panel, imageName);
            panel.BringToFront();

            mapa[y, x].EsZona = true;
            mapa[y, x].Nombre = $"Enemigo: {tipo}";
            mapa[y, x].IDEnemigo = idEnemigoDB;
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
            mapaPecio.Controls.Add(visual);
            jugador.Visual.BringToFront();
        }

        private void MarcarObstaculosEjemplo(int startX, int startY)
        {
            int[][] muros = new int[][]
            {
                new int[] {0,0}, new int[] {0,1}, new int[] {0,2}, new int[] {0,3}, new int[] {0,4},
                new int[] {0,5}, new int[] {0,6}, new int[] {0,7}, new int[] {0,8}, new int[] {0,9},
                new int[] {0,11}, new int[] {0,12}, new int[] {0,13}, new int[] {0,14},
                new int[] {0,15}, new int[] {0,16}, new int[] {0,17}, new int[] {0,18}, new int[] {0,19},
                new int[] {11,0}, new int[] {11,1}, new int[] {11,2}, new int[] {11,3}, new int[] {11,4},
                new int[] {11,5}, new int[] {11,6}, new int[] {11,7}, new int[] {11,8}, new int[] {11,9},
                new int[] {11,12}, new int[] {11,13}, new int[] {11,14},
                new int[] {11,15}, new int[] {11,16}, new int[] {11,17}, new int[] {11,18}, new int[] {11,19},
                new int[] {1,2}, new int[] {2,2}, new int[] {3,2}, new int[] {4,2}, new int[] {5,2}, new int[] {6,2},
                new int[] {1,8}, new int[] {2,8}, new int[] {3,8}, new int[] {4,8}, new int[] {5,8}, new int[] {6,8},
                new int[] {1,12}, new int[] {2,12}, new int[] {3,12}, new int[] {4,12}, new int[] {5,12}, new int[] {6,12},
                new int[] {1,18}, new int[] {2,18}, new int[] {3,18}, new int[] {4,18}, new int[] {5,18}, new int[] {6,18},
                new int[] {2,3}, new int[] {2,4}, new int[] {2,6}, new int[] {2,7},
                new int[] {5,3}, new int[] {5,4}, new int[] {5,6}, new int[] {5,7},
                new int[] {2,13}, new int[] {2,14}, new int[] {2,16}, new int[] {2,17},
                new int[] {5,13}, new int[] {5,14}, new int[] {5,16}, new int[] {5,17},
                new int[] {7,3}, new int[] {7,4}, new int[] {7,5},
                new int[] {7,13}, new int[] {7,14}, new int[] {7,15},
                new int[] {8,7}, new int[] {8,8}, new int[] {8,9},
                new int[] {8,12}, new int[] {8,13}, new int[] {8,14}
            };

            foreach (var pos in muros)
            {
                int y = pos[0], x = pos[1];
                if (y < 0 || y >= filas || x < 0 || x >= columnas) continue;
                if (mapa[y, x].EsZona) continue;
                if (y == startY && x == startX) continue;

                mapa[y, x].EsObstaculo = true;
                mapa[y, x].Nombre = "Muro";
                SetFondo(mapa[y, x].PanelVisual, "madera.png");
            }

            var libres = new (int y, int x)[]
            {
                (9, 0), (9, 1), (9, 2), (9, 3), (9, 4), (9, 5), (9, 6), (9, 7),
                (8, 0), (8, 1), (10, 0), (10, 1)
            };

            foreach (var c in libres)
            {
                if (c.y >= 0 && c.y < filas && c.x >= 0 && c.x < columnas)
                {
                    if (mapa[c.y, c.x].EsZona) continue;

                    mapa[c.y, c.x].EsObstaculo = false;
                    SetFondo(mapa[c.y, c.x].PanelVisual, "agua.png");
                    mapa[c.y, c.x].Nombre = $"Pecio {c.x},{c.y}";
                }
            }
        }

        private string GenerarDescripcionZona(Tile tile)
        {
            if (tile.Nombre.StartsWith("Zona C1")) return "Cofa quebrada. El mástil apunta al vacío. Nada yace aquí... por ahora.";
            if (tile.Nombre.StartsWith("Zona C2")) return "Jarcias rotas. La balista oxidada no responde. Silencio absoluto.";
            if (tile.Nombre.StartsWith("Zona C3")) return "Instrumentos náuticos olvidados. Un cofre reposa, intacto. Tesoro o trampa?";
            if (tile.Nombre.StartsWith("Zona C4")) return "Puerta atrancada. El hedor a muerte es real. Zombis aguardan sin duda.";
            if (tile.Nombre.StartsWith("Zona C5")) return "Ollas vacías. Restos de festines antiguos. Nada útil... por ahora.";
            if (tile.Nombre.StartsWith("Zona C6")) return "Camastros podridos. El tablón suelto no engaña. Trampa asegurada.";
            if (tile.Nombre.StartsWith("Zona C7")) return "Platos servidos. Vajilla intacta. Pero el alma del comedor se ha ido.";
            if (tile.Nombre.StartsWith("Zona C8")) return "Escombros flotan. Un cofre sumergido espera. Rescate o condena?";
            if (tile.Nombre.StartsWith("Zona C9")) return "Burbuja púrpura. El cofre del capitán respira. Talismán oculto en su vientre.";
            if (tile.Nombre.StartsWith("Zona C10")) return "Astrolabio suspendido. El aire vibra. Algo se aproxima... enemigo final?";
            if (tile.Nombre.StartsWith("Enemigo: Estirge")) return "Estirge: criatura alada, sedienta de sangre. Acecha desde las sombras.";
            if (tile.Nombre.StartsWith("Enemigo: Hongo violeta")) return "Hongo violeta: emite esporas paralizantes. Su cuerpo vibra con veneno.";
            if (tile.Nombre.StartsWith("Enemigo: Draco de humo")) return "Draco de humo: elemental de gas tóxico. Su aliento es fuego y olvido.";
            return "Casilla sin eventos especiales.";
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

            if (jugador == null) return base.ProcessCmdKey(ref msg, keyData);
            int nuevoX = jugador.X;
            int nuevoY = jugador.Y;
            bool movio = false;

            switch (keyData)
            {
                case Keys.W: nuevoY--; movio = true; break;
                case Keys.S: nuevoY++; movio = true; break;
                case Keys.A: nuevoX--; movio = true; break;
                case Keys.D: nuevoX++; movio = true; break;
            }

            if (!movio) return base.ProcessCmdKey(ref msg, keyData);

            if (nuevoX >= 0 && nuevoX < columnas && nuevoY >= 0 && nuevoY < filas)
            {
                Tile destino = mapa[nuevoY, nuevoX];
                if (!destino.EsObstaculo)
                {
                    jugador.X = nuevoX;
                    jugador.Y = nuevoY;
                    jugador.Visual.Location = destino.PanelVisual.Location;
                    jugador.Visual.BringToFront();
                    mapaPecio.ScrollControlIntoView(jugador.Visual);

                    if (jugador.X == 0 && jugador.Y == 9)
                    {
                        MessageBox.Show("Regresando al Retiro del Dragón...", "Transición", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Form5 retiro = new Form5();
                        retiro.Show();
                        retiro.MoverJugadorA(0, 0);
                        this.Close();
                        return true;
                    }
                }
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
                    SetFondo(tileActual.PanelVisual, "agua.png");
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

            return base.ProcessCmdKey(ref msg, keyData);
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

        private void SetFondo(Panel panel, string nombreImagen)
        {
            try
            {
                Image imagen = CargarImagenDesdeRecursos(nombreImagen);
                panel.BackgroundImage = imagen;
                panel.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { panel.BackColor = Color.DarkSlateGray; }
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
    }
}