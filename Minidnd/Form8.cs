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
    public partial class Form8 : Form
    {
        private Panel mapaObservatorio;
        private const int Tamano = 50;
        private const int filas = 12;
        private const int columnas = 25;
        private Tile[,] mapa = new Tile[filas, columnas];
        private Personaje jugador;
        private int enemigoD3 = 0;
        private int usuarioId = Form9.JugadorIdActual;
        private Label lblEstadoJugador;
        private Button btnInventario, btnSalir, btnLogros, btnEstadisticas, btnDescanso;
        private bool botonesVisibles = true;
        private SoundPlayer musicaFondo;

        public Form8()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            musicaFondo = new SoundPlayer("Musica.wav");
            musicaFondo.PlayLooping();

            mapaObservatorio = new Panel
            {
                AutoScroll = true,
                Size = this.ClientSize,
                Location = new Point(0, 0)
            };
            this.Controls.Add(mapaObservatorio);
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
            this.BackgroundImage = Image.FromFile("map4.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Text = "Dungeons && Dragons - Observatorio";

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

                    mapaObservatorio.Controls.Add(panel);

                    mapa[y, x] = new Tile
                    {
                        X = x,
                        Y = y,
                        PanelVisual = panel,
                        EsZona = false,
                        EsObstaculo = false,
                        Nombre = $"Observatorio {x},{y}",
                        IDEnemigo = 0
                    };
                }
            }
            SetFondo(mapa[3, 2].PanelVisual, "entrada2.png");

            // Enemigos con IDs: Kobold (ID 13), Estirge (ID 18), Aidron (ID 21)
            SetEnemigo(3, 3, "Kobold guardia", "Azul.png", 13);
            SetEnemigo(3, 11, "Estirge", "bicho.png", 18);
            SetEnemigo(3, 17, "Aidron", "runara.png", 21);

            int[][] muros = new int[][]
            {
                new int[] {1,1}, new int[] {1,2}, new int[] {1,3}, new int[] {1,4}, new int[] {1,5},
                new int[] {2,1}, new int[] {2,5},
                new int[] {3,1}, new int[] {5,3},
                new int[] {4,1}, new int[] {4,5},
                new int[] {5,1}, new int[] {5,2}, new int[] {5,4}, new int[] {5,5},
                new int[] {2,6}, new int[] {2,7}, new int[] {2,8},
                new int[] {4,6}, new int[] {4,7}, new int[] {4,8},
                new int[] {0,9}, new int[] {0,10}, new int[] {0,11}, new int[] {0,12},
                new int[] {1,8}, new int[] {1,9}, new int[] {1,12}, new int[] {1,13},
                new int[] {2,8}, new int[] {2,13},
                new int[] {4,8}, new int[] {4,13},
                new int[] {5,9}, new int[] {5,12},
                new int[] {6,9}, new int[] {6,10}, new int[] {6,11}, new int[] {6,12},
                new int[] {2,13}, new int[] {2,14},
                new int[] {4,13}, new int[] {4,14},
                new int[] {0,15}, new int[] {0,16}, new int[] {0,17}, new int[] {0,18},
                new int[] {1,14}, new int[] {1,15}, new int[] {1,18}, new int[] {1,19},
                new int[] {2,14}, new int[] {2,19}, new int[] {3,19},
                new int[] {4,14}, new int[] {4,19},
                new int[] {5,15}, new int[]{5,16}, new int[]{5,17}, new int[] {5,18}
            };

            foreach (var pos in muros)
            {
                int y = pos[0], x = pos[1];
                if (y < 0 || y >= filas || x < 0 || x >= columnas) continue;
                if (mapa[y, x].EsZona) continue;

                mapa[y, x].EsObstaculo = true;
                mapa[y, x].Nombre = "Muralla de piedra";
                SetFondoSimple(mapa[y, x].PanelVisual, "muro.png");
            }

            int[][] pasillos = new int[][]
            {
                new int[] {3,6}, new int[] {3,7}, new int[] {3,8}, new int[] {3,9},
                new int[] {3,13}, new int[] {3,14}, new int[] {3,15}
            };

            foreach (var pos in pasillos)
            {
                int y = pos[0], x = pos[1];
                if (y >= 0 && y < filas && x >= 0 && x < columnas && !mapa[y, x].EsZona)
                {
                    SetFondoSimple(mapa[y, x].PanelVisual, "pasillo.png");
                }
            }

            int[][] interior = new int[][]
            {
                new int[] {2,2}, new int[] {2,3}, new int[] {2,4},
                new int[] {3,2}, new int[] {3,3}, new int[] {3,4}, new int[] {3,5},
                new int[] {4,2}, new int[] {4,3}, new int[] {4,4},
                new int[] {1,10}, new int[] {1,11},
                new int[] {2,9}, new int[] {2,10}, new int[] {2,11}, new int[] {2,12},
                new int[] {3,10}, new int[] {3,11}, new int[] {3,12},
                new int[] {4,9}, new int[] {4,10}, new int[] {4,11}, new int[] {4,12},
                new int[] {1,16}, new int[] {1,17},
                new int[] {2,15}, new int[] {2,16}, new int[] {2,17}, new int[] {2,18},
                new int[] {3,16}, new int[] {3,17}, new int[] {3,18},
                new int[] {4,15}, new int[] {4,16}, new int[] {4,17}, new int[] {4,18}
            };

            foreach (var pos in interior)
            {
                int y = pos[0], x = pos[1];
                if (y >= 0 && y < filas && x >= 0 && x < columnas && !mapa[y, x].EsZona && !mapa[y, x].EsObstaculo)
                {
                    SetFondoSimple(mapa[y, x].PanelVisual, "piedra.png");
                }
            }
        }

        private void SetEnemigo(int y, int x, string tipo, string imageName, int idEnemigoDB)
        {
            if (y < 0 || y >= filas || x < 0 || x >= columnas) return;

            var panel = mapa[y, x].PanelVisual;

            try
            {
                Image fondoAgua = CargarImagenDesdeRecursos("agua.png");
                Image imagenEnemigo = CargarImagenDesdeRecursos(imageName);

                Bitmap combinado = new Bitmap(Tamano, Tamano);
                using (Graphics g = Graphics.FromImage(combinado))
                {
                    g.DrawImage(fondoAgua, 0, 0, Tamano, Tamano);
                    g.DrawImage(imagenEnemigo, 0, 0, Tamano, Tamano);
                }

                panel.BackgroundImage = combinado;
                panel.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch
            {
                panel.BackColor = Color.Red;
            }

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
            mapaObservatorio.Controls.Add(visual);
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
                    mapaObservatorio.ScrollControlIntoView(jugador.Visual);

                    if (jugador.X == 2 && jugador.Y == 3)
                    {
                        MessageBox.Show("Regresando al Retiro del Dragón...", "Transición", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Form5 retiro = new Form5();
                        retiro.Show();
                        retiro.MoverJugadorA(39, 12);
                        this.Close();
                        return true;
                    }
                }
            }

            // SISTEMA DE COMBATE CON BOSS PROGRESSION
            var tileActual = mapa[jugador.Y, jugador.X];
            if (tileActual.Nombre.StartsWith("Enemigo:") && tileActual.IDEnemigo > 0)
            {
                string enemigo = tileActual.Nombre.Replace("Enemigo: ", "");

                // Enemigos normales: Kobold y Estirge
                if (enemigo == "Kobold guardia" || enemigo == "Estirge")
                {
                    this.Hide();
                    Form16 combate = new Form16(tileActual.IDEnemigo, this);
                    DialogResult resultado = combate.ShowDialog();

                    if (resultado == DialogResult.OK)
                    {
                        SetFondoSimple(tileActual.PanelVisual, "agua.png");
                        tileActual.Nombre = $"Observatorio {tileActual.X},{tileActual.Y}";
                        tileActual.EsZona = false;
                        tileActual.IDEnemigo = 0;
                    }

                    ActualizarEstadoUI();
                    this.Show();
                }
                // Boss progression: Aidron -> Chispa -> Dragón Espiritual
                else if (enemigo == "Aidron" && enemigoD3 == 0)
                {
                    this.Hide();
                    Form16 combate = new Form16(tileActual.IDEnemigo, this);
                    DialogResult resultado = combate.ShowDialog();

                    if (resultado == DialogResult.OK)
                    {
                        enemigoD3 = 1;
                        // Cambiar a Chispa Fulminante (ID 22 según tu DB)
                        SetEnemigoEnTile(tileActual, "dragonfuegp.png", 22);
                        tileActual.Nombre = "Enemigo: Chispa Fulminante";
                        MessageBox.Show("Aparece Chispa Fulminante. Rayos eléctricos llenan el aire.", "Nuevo enemigo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    ActualizarEstadoUI();
                    this.Show();
                }
                else if (enemigo == "Chispa Fulminante" && enemigoD3 == 1)
                {
                    this.Hide();
                    Form16 combate = new Form16(tileActual.IDEnemigo, this);
                    DialogResult resultado = combate.ShowDialog();

                    if (resultado == DialogResult.OK)
                    {
                        enemigoD3 = 2;
                        // Cambiar a Dragón Espiritual (ID 23 según tu DB)
                        SetEnemigoEnTile(tileActual, "dragonhumo.png", 23);
                        tileActual.Nombre = "Enemigo: Dragón Espiritual";
                        MessageBox.Show("El Dragón Espiritual emerge. Su rugido sacude la torre.", "Boss Final", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    ActualizarEstadoUI();
                    this.Show();
                }
                else if (enemigo == "Dragón Espiritual" && enemigoD3 == 2)
                {
                    this.Hide();
                    Form16 combate = new Form16(tileActual.IDEnemigo, this);
                    DialogResult resultado = combate.ShowDialog();

                    if (resultado == DialogResult.OK)
                    {
                        MessageBox.Show("¡Has conquistado el Observatorio!", "VICTORIA FINAL", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        SetFondoSimple(tileActual.PanelVisual, "agua.png");
                        tileActual.Nombre = $"Observatorio {tileActual.X},{tileActual.Y}";
                        tileActual.EsZona = false;
                        tileActual.IDEnemigo = 0;
                    }

                    ActualizarEstadoUI();
                    this.Show();
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetEnemigoEnTile(Tile tile, string imageName, int idEnemigoDB)
        {
            try
            {
                Image fondoAgua = CargarImagenDesdeRecursos("agua.png");
                Image imagenEnemigo = CargarImagenDesdeRecursos(imageName);

                Bitmap combinado = new Bitmap(Tamano, Tamano);
                using (Graphics g = Graphics.FromImage(combinado))
                {
                    g.DrawImage(fondoAgua, 0, 0, Tamano, Tamano);
                    g.DrawImage(imagenEnemigo, 0, 0, Tamano, Tamano);
                }

                tile.PanelVisual.BackgroundImage = combinado;
                tile.PanelVisual.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch
            {
                tile.PanelVisual.BackColor = Color.Red;
            }

            tile.IDEnemigo = idEnemigoDB;
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

        private void SetFondoSimple(Panel panel, string nombreImagen)
        {
            try
            {
                Image imagen = CargarImagenDesdeRecursos(nombreImagen);
                panel.BackgroundImage = imagen;
                panel.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { panel.BackColor = Color.DarkSlateBlue; }
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