using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static Proyecto_Dnd.Form5;
using static Proyecto_Dnd.Form10;
using System.Reflection;
using System.Linq;
using System.Media;
using System.Data;

namespace Proyecto_Dnd
{
    public partial class Form6 : Form
    {
        private Panel mapaCuevas;
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

        public Form6()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
            this.Activated += (s, e) => this.Focus();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            musicaFondo = new SoundPlayer("Musica.wav");
            musicaFondo.PlayLooping();
            mapaCuevas = new Panel
            {
                AutoScroll = true,
                Size = this.ClientSize,
                Location = new Point(0, 0)
            };
            this.Controls.Add(mapaCuevas);
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
            this.BackgroundImage = Image.FromFile("map2.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Text = "Dungeons && Dragons - Cuevas Marinas";

            for (int y = 0; y < filas; y++)
            {
                for (int x = 0; x < columnas; x++)
                {
                    Panel panel = new Panel
                    {
                        Size = new Size(Tamano, Tamano),
                        Location = new Point(x * Tamano, y * Tamano),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackgroundImage = CargarImagenDesdeRecursos("roca.png"),
                        BackgroundImageLayout = ImageLayout.Stretch,
                    };

                    mapaCuevas.Controls.Add(panel);

                    mapa[y, x] = new Tile
                    {
                        X = x,
                        Y = y,
                        PanelVisual = panel,
                        EsZona = false,
                        EsObstaculo = false,
                        Nombre = $"Cueva {x},{y}",
                        IDEnemigo = 0
                    };
                }
            }

            SetFondo(mapa[9, 1].PanelVisual, "entrada1.png");
            SetZona(0, 10, "Zona B1 – Túnel de entrada");
            SetZona(2, 10, "Zona B2 – Granja de hongos");
            SetZona(4, 8, "Zona B3 – Despensa");
            SetZona(6, 10, "Zona B4 – Cámara circular");
            SetZona(8, 12, "Zona B5 – Santuario de Sinensa");
            SetZona(10, 18, "Zona B6 – Cueva del cristal");

            int[][] muros = new int[][]
            {
                new int[] {0,0}, new int[] {0,1}, new int[] {0,2}, new int[] {0,3}, new int[] {0,4},
                new int[] {0,5}, new int[] {0,6}, new int[] {0,7}, new int[] {0,8}, new int[] {0,9},
                new int[] {0,12}, new int[] {0,13}, new int[] {0,14},
                new int[] {0,15}, new int[] {0,16}, new int[] {0,17}, new int[] {0,18}, new int[] {0,19},
                new int[] {11,0}, new int[] {11,1}, new int[] {11,2}, new int[] {11,3}, new int[] {11,4},
                new int[] {11,5}, new int[] {11,6}, new int[] {11,7}, new int[] {11,8}, new int[] {11,9},
                new int[] {11,10}, new int[] {11,11}, new int[] {11,12}, new int[] {11,13}, new int[] {11,14},
                new int[] {11,15}, new int[] {11,16}, new int[] {11,17}, new int[] {11,18}, new int[] {11,19},
                new int[] {1,0}, new int[] {2,0}, new int[] {3,0}, new int[] {4,0}, new int[] {5,0},
                new int[] {6,0}, new int[] {7,0}, new int[] {8,0}, new int[] {9,0}, new int[] {10,0},
                new int[] {1,19}, new int[] {2,19}, new int[] {3,19}, new int[] {4,19}, new int[] {5,19},
                new int[] {6,19}, new int[] {7,19}, new int[] {8,19}, new int[] {9,19}, new int[] {10,19},
                new int[] {8,1}, new int[] {8,2}, new int[] {8,3}, new int[] {8,4},
                new int[] {10,1}, new int[] {10,2}, new int[] {10,3}, new int[] {10,4}, new int[] {10,5}, new int[] {10,6},
                new int[] {2,2}, new int[] {3,2}, new int[] {4,2}, new int[] {5,2}, new int[] {6,2},
                new int[] {2,4}, new int[] {3,4}, new int[] {4,4}, new int[] {5,4}, new int[] {6,4}, new int[] {7,4},
                new int[] {4,9}, new int[] {4,10}, new int[] {4,11},
                new int[] {6,8}, new int[] {7,8},
                new int[] {5,12}, new int[] {7,12},
                new int[] {2,7},
                new int[] {6,7}, new int[] {6,9},
                new int[] {3,6}, new int[] {5,6},
                new int[] {3,10}, new int[] {4,10}, new int[] {5,10},
                new int[] {4,13}, new int[] {4,14}, new int[] {4,15},
                new int[] {8,13}, new int[] {8,14}, new int[] {8,15},
                new int[] {5,11}, new int[] {6,11}, new int[] {7,11},
                new int[] {5,16}, new int[] {7,16},
                new int[] {7,12}, new int[] {7,13}, new int[] {7,14}, new int[] {7,15},
                new int[] {11,12}, new int[] {11,13}, new int[] {11,14}, new int[] {11,15},
                new int[] {9,11}, new int[] {10,11},
                new int[] {7,16}, new int[] {7,17}, new int[] {7,18},
                new int[] {11,16}, new int[] {11,17},
                new int[] {8,17}, new int[] {10,17},
                new int[] {2,5}, new int[] {2,6}, new int[] {2,7},
                new int[] {4,1}, new int[] {5,1}, new int[] {6,1},
                new int[] {2,13}, new int[] {2,14}, new int[] {2,15},
                new int[] {9,8}, new int[] {10,8}, new int[] {10,9}
            };

            foreach (var pos in muros)
            {
                int y = pos[0], x = pos[1];
                mapa[y, x].EsObstaculo = true;
                mapa[y, x].Nombre = "Muro fúngico";
                SetFondo(mapa[y, x].PanelVisual, "montana.png");
            }

            // Enemigos con IDs: Zombi (1), Hongo violeta (12), Draco de humo (20)
            SetEnemigo(2, 9, "Zombie", "zombie.png", 1);
            SetEnemigo(4, 6, "Hongo violeta", "hongoB.png", 12);
            SetEnemigo(6, 12, "Hongo violeta", "hongoB.png", 12);
            SetEnemigo(8, 11, "Zombie", "zombie.png", 1);
            SetEnemigo(10, 18, "Draco de humo", "dragonhumo.png", 20);
        }

        private void SetZona(int y, int x, string nombre)
        {
            var panel = mapa[y, x].PanelVisual;
            SetFondo(panel, "zona.png");
            mapa[y, x].EsZona = true;
            mapa[y, x].Nombre = nombre;
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
            mapaCuevas.Controls.Add(visual);
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
                    mapaCuevas.ScrollControlIntoView(jugador.Visual);

                    if (jugador.X == 1 && jugador.Y == 9)
                    {
                        MessageBox.Show("Regresando al Retiro del Dragón...", "Transición", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Form5 retiro = new Form5();
                        retiro.Show();
                        retiro.MoverJugadorA(0, 23);
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
                    SetFondo(tileActual.PanelVisual, "roca.png");
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

        private void ActivarZona(Tile tile)
        {
            switch (tile.Nombre)
            {
                case "Zona B1 – Túnel de entrada":
                    MessageBox.Show("Un túnel bioluminiscente se extiende ante ti. Esporas flotan en el agua.", "Zona B1", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("Un pulpo espora emerge del agua y ataca a los intrusos.", "Guardia fúngico", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    SetFondo(tile.PanelVisual, "roca.png");
                    tile.Nombre = $"Cueva {tile.X},{tile.Y}";
                    tile.EsZona = false;
                    break;

                case "Zona B2 – Granja de hongos":
                    MessageBox.Show("Un bosque de hongos multicolores se extiende ante ti. Dos brotes micónidos trabajan junto al estanque.", "Zona B2", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("Un hongo violeta cobra vida y extiende zarcillos morados hacia ti.", "Ataque fúngico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetFondo(tile.PanelVisual, "roca.png");
                    tile.Nombre = $"Cueva {tile.X},{tile.Y}";
                    tile.EsZona = false;
                    break;

                case "Zona B3 – Despensa":
                    MessageBox.Show("La cueva apesta a podredumbre. Tres brotes micónidos trabajan entre vegetación marchita.", "Zona B3", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("Una estructura gelatinosa reluce en la pared... es un nido de estirges.", "Peligro aéreo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    SetFondo(tile.PanelVisual, "roca.png");
                    tile.Nombre = $"Cueva {tile.X},{tile.Y}";
                    tile.EsZona = false;
                    break;

                case "Zona B4 – Cámara circular":
                    MessageBox.Show("Seis cúmulos de hongos gigantes forman un círculo. Micónidos en trance meditan en el centro.", "Zona B4", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("Dos micónidos conscientes se acercan con actitud defensiva.", "Reacción hostil", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetFondo(tile.PanelVisual, "roca.png");
                    tile.Nombre = $"Cueva {tile.X},{tile.Y}";
                    tile.EsZona = false;
                    break;

                case "Zona B5 – Santuario de Sinensa":
                    MessageBox.Show("Cristales brillantes y hongos rojos iluminan la cueva. En el centro, Sinensa yace inconsciente.", "Zona B5", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("Los micónidos cuidan de su líder con esporas curativas. Defienden el santuario con vehemencia.", "Protección fúngica", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetFondo(tile.PanelVisual, "roca.png");
                    tile.Nombre = $"Cueva {tile.X},{tile.Y}";
                    tile.EsZona = false;
                    break;

                case "Zona B6 – Cueva del cristal":
                    MessageBox.Show("El aire está impregnado de humo sulfúrico. Cristales morados y un núcleo naranja iluminan la cueva.", "Zona B6", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("Dos dracos de humo emergen entre los gases y atacan a los intrusos.", "Combate elemental", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    MessageBox.Show("Si destruyes el cristal naranja, liberarás los gases y acabarás con la plaga.", "Cristal de fuego", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SetFondo(tile.PanelVisual, "roca.png");
                    tile.Nombre = $"Cueva {tile.X},{tile.Y}";
                    tile.EsZona = false;
                    break;
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

        private void SetFondo(Panel panel, string nombreImagen)
        {
            try
            {
                Image imagen = CargarImagenDesdeRecursos(nombreImagen);
                panel.BackgroundImage = imagen;
                panel.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { panel.BackColor = Color.DimGray; }
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