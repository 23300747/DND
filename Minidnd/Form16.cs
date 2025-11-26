using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Reflection;
using static Proyecto_Dnd.Form16;

namespace Proyecto_Dnd
{
    public partial class Form16 : Form
    {
        private int usuarioId = Form9.JugadorIdActual;
        private Jugador jugador;
        private Enemigo enemigoActual;
        private bool turnoJugador = true;
        private int turnoNumero = 1;
        private int danoTotalInfligido = 0;
        private int danoTotalRecibido = 0;

        private Label lblHPJugador;
        private Label lblHPEnemigo;
        private Label lblEstadoCombate;
        private Label lblTurno;
        private Panel zonaJugador;
        private Panel zonaEnemigo;
        private Button[] botonesAccion;

        private int idEnemigoBase;
        private Form formularioOrigen;

        private int criticosJugador = 0;
        private int expGanada = 0;
        private int oroGanado = 0;
        private bool subioNivel = false;
        public Form16(int idEnemigo, Form origen)
        {
            InitializeComponent();
            this.idEnemigoBase = idEnemigo;
            this.formularioOrigen = origen;
            ConfigurarPantallaCombate();
            CargarDatosJugador();
            CargarEnemigo(idEnemigo);
            IniciarCombate();
        }

        private void Form16_Load(object sender, EventArgs e)
        {
        }

        private void ConfigurarPantallaCombate()
        {
            this.Text = "Combate D&D";
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            zonaJugador = new Panel()
            {
                Size = new Size(180, 180),
                Location = new Point(150, this.ClientSize.Height - 350),
                BackColor = Color.FromArgb(100, 70, 90, 150),
                BorderStyle = BorderStyle.FixedSingle
            };

            zonaEnemigo = new Panel()
            {
                Size = new Size(180, 180),
                Location = new Point(this.ClientSize.Width - 330, 150),
                BackColor = Color.FromArgb(100, 150, 60, 60),
                BorderStyle = BorderStyle.FixedSingle
            };

            this.Controls.Add(zonaJugador);
            this.Controls.Add(zonaEnemigo);

            lblTurno = new Label()
            {
                Text = "Turno 1",
                ForeColor = Color.Gold,
                Font = new Font("Consolas", 16, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(200, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(this.ClientSize.Width / 2 - 100, 20),
                BackColor = Color.FromArgb(200, 30, 30, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblHPJugador = new Label()
            {
                Text = "HP: 100 / 100",
                ForeColor = Color.LimeGreen,
                Font = new Font("Consolas", 16, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(250, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(150, this.ClientSize.Height - 380),
                BackColor = Color.FromArgb(220, 10, 10, 10),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10, 8, 0, 0)
            };

            lblHPEnemigo = new Label()
            {
                Text = "HP: 100 / 100",
                ForeColor = Color.Red,
                Font = new Font("Consolas", 16, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(250, 40),
                TextAlign = ContentAlignment.MiddleRight,
                Location = new Point(this.ClientSize.Width - 400, 110),
                BackColor = Color.FromArgb(220, 10, 10, 10),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(0, 8, 10, 0)
            };

            lblEstadoCombate = new Label()
            {
                Text = "¡Comienza el combate!",
                ForeColor = Color.White,
                Font = new Font("Consolas", 14, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(800, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point((this.ClientSize.Width - 800) / 2, this.ClientSize.Height - 300),
                BackColor = Color.FromArgb(220, 20, 20, 20),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            this.Controls.Add(lblTurno);
            this.Controls.Add(lblHPJugador);
            this.Controls.Add(lblHPEnemigo);
            this.Controls.Add(lblEstadoCombate);

            lblTurno.BringToFront();
            lblHPJugador.BringToFront();
            lblHPEnemigo.BringToFront();
            lblEstadoCombate.BringToFront();

            CrearBotonesCombate();
        }

        private void CargarDatosJugador()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();
                    using (MySqlCommand cmd = new MySqlCommand("CALL CargarJugadorParaCombate(@JugadorId)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@JugadorId", usuarioId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int dadosGolpe = reader.GetInt32("DadosGolpe");
                                int nivel = reader.GetInt32("ID_Nivel");
                                int constitucion = reader.GetInt32("Constitucion");

                                jugador = new Jugador
                                {
                                    HP = reader.GetInt32("HP"),
                                    HPMaximo = dadosGolpe + (constitucion * nivel),
                                    Fuerza = reader.GetInt32("Fuerza"),
                                    Destreza = reader.GetInt32("Destreza"),
                                    Constitucion = constitucion,
                                    Inteligencia = reader.GetInt32("Inteligencia"),
                                    ClaseId = reader.GetInt32("ID_Clase"),
                                    Nombre = reader.GetString("Nombre"),
                                    Nivel = nivel
                                };
                            }
                        }
                    }
                }

                string rutaImagenJugador = "";
                switch (jugador.ClaseId)
                {
                    case 1: rutaImagenJugador = "guerrero.png"; break;
                    case 2: rutaImagenJugador = "mago.png"; break;
                    case 3: rutaImagenJugador = "paladin.png"; break;
                    case 4: rutaImagenJugador = "clerigo.png"; break;
                }

                Image imgJugador = CargarImagenDesdeRecursos(rutaImagenJugador);
                if (imgJugador != null)
                {
                    zonaJugador.BackgroundImage = imgJugador;
                    zonaJugador.BackgroundImageLayout = ImageLayout.Stretch;
                }

                ActualizarUIJugador();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar jugador: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarEnemigo(int idEnemigo)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();
                    using (MySqlCommand cmd = new MySqlCommand("CALL CargarEnemigoPorID(@IDEnemigo)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@IDEnemigo", idEnemigo);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                enemigoActual = new Enemigo
                                {
                                    ID = reader.GetInt32("ID_Enemigo"),
                                    Nombre = reader.GetString("Nombre"),
                                    HP = reader.GetInt32("HP"),
                                    HPMaximo = reader.GetInt32("HP"),
                                    Fuerza = reader.GetInt32("Fuerza"),
                                    Destreza = reader.GetInt32("Destreza"),
                                    Constitucion = reader.GetInt32("Constitucion"),
                                    Inteligencia = reader.GetInt32("Inteligencia"),
                                    Nivel = reader.GetInt32("Nivel")
                                };
                            }
                        }
                    }
                }

                string imagenEnemigo = ObtenerImagenEnemigo(enemigoActual.Nombre);
                Image imgEnemigo = CargarImagenDesdeRecursos(imagenEnemigo);
                if (imgEnemigo != null)
                {
                    zonaEnemigo.BackgroundImage = imgEnemigo;
                    zonaEnemigo.BackgroundImageLayout = ImageLayout.Stretch;
                }

                ActualizarUIEnemigo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar enemigo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ObtenerImagenEnemigo(string nombreEnemigo)
        {
            if (nombreEnemigo.Contains("Zombi") || nombreEnemigo.Contains("Ahogado")) return "zombi.png";
            if (nombreEnemigo.Contains("Pulpo")) return "pulpo.png";
            if (nombreEnemigo.Contains("Hongo")) return "hongoB.png";
            if (nombreEnemigo.Contains("Estirge")) return "bicho.png";
            if (nombreEnemigo.Contains("Draco")) return "dragonhumo.png";
            if (nombreEnemigo.Contains("Kobold")) return "kobold.png";
            if (nombreEnemigo.Contains("Aidron")) return "aidron.png";
            if (nombreEnemigo.Contains("Chispa")) return "chispa.png";
            if (nombreEnemigo.Contains("Dragón Espiritual")) return "dragonespiritual.png";
            return "zombi.png";
        }

        private void IniciarCombate()
        {
            lblEstadoCombate.Text = $"⚔️ INICIO DE COMBATE ⚔️\n\n" +
                $"Te enfrentas a:\n{enemigoActual.Nombre}\n\n" +
                $"¡Prepárate para luchar!";
            lblEstadoCombate.ForeColor = Color.Yellow;
        }

        private void CrearBotonesCombate()
        {
            botonesAccion = new Button[5];

            botonesAccion[0] = CrearBoton("Atacar", BtnAtacar_Click);
            botonesAccion[1] = CrearBoton("Habilidad", BtnHabilidad_Click);
            botonesAccion[2] = CrearBoton("Defender", BtnDefender_Click);
            botonesAccion[3] = CrearBoton("Inventario", BtnInventario_Click);
            botonesAccion[4] = CrearBoton("Huir", BtnHuir_Click);

            int espacio = 15;
            int anchoBoton = 110;
            int totalAncho = (anchoBoton * 5) + (espacio * 4);
            int xInicial = (this.ClientSize.Width - totalAncho) / 2;
            int yPos = this.ClientSize.Height - 100;

            for (int i = 0; i < botonesAccion.Length; i++)
            {
                botonesAccion[i].Location = new Point(xInicial + (i * (anchoBoton + espacio)), yPos);
                this.Controls.Add(botonesAccion[i]);
                botonesAccion[i].BringToFront();
            }
        }

        private Button CrearBoton(string texto, EventHandler clickHandler)
        {
            Button btn = new Button()
            {
                Text = texto,
                Size = new Size(110, 60),
                BackColor = Color.FromArgb(139, 69, 19),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Papyrus", 11, FontStyle.Bold)
            };

            btn.FlatAppearance.BorderColor = Color.Gold;
            btn.FlatAppearance.BorderSize = 2;
            btn.Click += clickHandler;

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(160, 82, 45);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(139, 69, 19);

            return btn;
        }

        private async void BtnAtacar_Click(object sender, EventArgs e)
        {
            if (!turnoJugador) return;

            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {// CONTINUACIÓN DE BtnAtacar_Click (desde donde se cortó)
                    conexion.Open();
                    using (MySqlCommand cmd = new MySqlCommand("CALL RealizarAtaqueConDados(@Atacante, @HPDefensor, @FuerzaAtacante, @DestrezaDefensor, @ConstitucionDefensor, @Dado, @DanoFinal, @HPRestante, @EsCritico, @Mensaje)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@Atacante", jugador.Nombre);
                        cmd.Parameters.AddWithValue("@HPDefensor", enemigoActual.HP);
                        cmd.Parameters.AddWithValue("@FuerzaAtacante", jugador.Fuerza);
                        cmd.Parameters.AddWithValue("@DestrezaDefensor", enemigoActual.Destreza);
                        cmd.Parameters.AddWithValue("@ConstitucionDefensor", enemigoActual.Constitucion);

                        cmd.Parameters.Add("@Dado", MySqlDbType.Int32).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add("@DanoFinal", MySqlDbType.Int32).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add("@HPRestante", MySqlDbType.Int32).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add("@EsCritico", MySqlDbType.Bit).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add("@Mensaje", MySqlDbType.VarChar, 200).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        int dano = Convert.ToInt32(cmd.Parameters["@DanoFinal"].Value);
                        int hpRestante = Convert.ToInt32(cmd.Parameters["@HPRestante"].Value);
                        bool critico = Convert.ToBoolean(cmd.Parameters["@EsCritico"].Value);
                        int dado = Convert.ToInt32(cmd.Parameters["@Dado"].Value);

                        enemigoActual.HP = hpRestante;
                        danoTotalInfligido += dano;

                        if (dano > 0)
                        {
                            if (critico)
                            {
                                lblEstadoCombate.Text = $"🎯 ¡GOLPE CRÍTICO! 🎯\n\n" +
                                    $"{jugador.Nombre} lanza un ataque devastador\n" +
                                    $"D20: {dado} (CRÍTICO)\n" +
                                    $"Daño: {dano} HP\n\n" +
                                    $"¡{enemigoActual.Nombre} recibe un golpe mortal!";
                                lblEstadoCombate.ForeColor = Color.Gold;
                            }
                            else
                            {
                                lblEstadoCombate.Text = $"⚔️ ATAQUE EXITOSO ⚔️\n\n" +
                                    $"{jugador.Nombre} golpea a {enemigoActual.Nombre}\n" +
                                    $"D20: {dado}\n" +
                                    $"Daño: {dano} HP\n\n" +
                                    $"HP Enemigo: {enemigoActual.HP}/{enemigoActual.HPMaximo}";
                                lblEstadoCombate.ForeColor = Color.LightGreen;
                            }
                        }
                        else
                        {
                            lblEstadoCombate.Text = $"❌ ATAQUE FALLIDO ❌\n\n" +
                                $"{jugador.Nombre} intenta golpear\n" +
                                $"D20: {dado}\n\n" +
                                $"¡{enemigoActual.Nombre} esquiva el ataque!";
                            lblEstadoCombate.ForeColor = Color.Orange;
                        }

                        ActualizarUIEnemigo();
                    }
                }

                System.Threading.Thread.Sleep(1500);

                if (!VerificarFinCombate())
                {
                    await TurnoEnemigo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en ataque: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnHabilidad_Click(object sender, EventArgs e)
        {
            if (!turnoJugador) return;

            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    bool esClaseMagica = (jugador.ClaseId == 2 || jugador.ClaseId == 4);
                    List<dynamic> habilidades = new List<dynamic>();

                    if (esClaseMagica)
                    {
                        // LLAMAR AL STORED PROCEDURE PARA HECHIZOS
                        using (MySqlCommand cmd = new MySqlCommand("CALL CargarHechizosJugador(@clase, @nivel)", conexion))
                        {
                            cmd.Parameters.AddWithValue("@clase", jugador.ClaseId);
                            cmd.Parameters.AddWithValue("@nivel", jugador.Nivel);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    habilidades.Add(new
                                    {
                                        Id = reader.GetInt32("ID_Hechizo"),
                                        Nombre = reader.GetString("Nombre"),
                                        Descripcion = reader.GetString("Descripcion"),
                                        Dano = reader.GetInt32("DMG"),
                                        Nivel = reader.GetInt32("ID_Nivel"),
                                        EsHechizo = true
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        // LLAMAR AL STORED PROCEDURE PARA HABILIDADES
                        using (MySqlCommand cmd = new MySqlCommand("CALL CargarHabilidadesJugador(@clase, @nivel)", conexion))
                        {
                            cmd.Parameters.AddWithValue("@clase", jugador.ClaseId);
                            cmd.Parameters.AddWithValue("@nivel", jugador.Nivel);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int idHab = reader.GetInt32("ID_Habilidades");
                                    habilidades.Add(new
                                    {
                                        Id = idHab,
                                        Nombre = reader.GetString("Nombre"),
                                        Descripcion = reader.GetString("Descripcion"),
                                        Dano = ObtenerDanoHabilidad(idHab),
                                        Nivel = reader.GetInt32("NivelRequerido"),
                                        EsHechizo = false
                                    });
                                }
                            }
                        }
                    }

                    if (habilidades.Count == 0)
                    {
                        MessageBox.Show(
                            "No tienes habilidades/hechizos desbloqueados para tu nivel.",
                            "Sin habilidades",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }

                    // CREAR FORMULARIO DE SELECCIÓN
                    Form formHabilidades = new Form
                    {
                        Text = esClaseMagica ? "✨ Hechizos" : "⚔️ Habilidades",
                        Size = new Size(650, 580),
                        StartPosition = FormStartPosition.CenterParent,
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        MaximizeBox = false,
                        MinimizeBox = false,
                        BackColor = Color.FromArgb(40, 30, 20)
                    };

                    Label lblTitulo = new Label
                    {
                        Text = esClaseMagica ? "✨ GRIMORIO ✨" : "⚔️ HABILIDADES ⚔️",
                        Font = new Font("Papyrus", 16, FontStyle.Bold),
                        ForeColor = Color.Gold,
                        Location = new Point(20, 15),
                        Size = new Size(600, 40),
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    ListBox lista = new ListBox
                    {
                        Location = new Point(20, 70),
                        Size = new Size(600, 300),
                        Font = new Font("Consolas", 11),
                        BackColor = Color.FromArgb(240, 235, 220)
                    };

                    foreach (var hab in habilidades)
                    {
                        string efecto = hab.Dano > 0 ? $"[DMG:{hab.Dano}]" :
                                       (hab.Dano < 0 ? $"[CURA:{-hab.Dano}]" : "[BUFF]");
                        lista.Items.Add($"[Nv.{hab.Nivel}] {hab.Nombre} {efecto}");
                    }

                    Label lblDesc = new Label
                    {
                        Text = "Selecciona una opción para ver detalles",
                        Location = new Point(20, 385),
                        Size = new Size(600, 100),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 10),
                        Padding = new Padding(10),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    lista.SelectedIndexChanged += (s, ev) =>
                    {
                        if (lista.SelectedIndex >= 0)
                        {
                            var hab = habilidades[lista.SelectedIndex];
                            string tipo = hab.Dano > 0 ? $"{hab.Dano} de daño" :
                                         (hab.Dano < 0 ? $"{-hab.Dano} HP curación" : "Buff temporal");
                            lblDesc.Text = $"📖 {hab.Descripcion}\n\nEfecto: {tipo}";
                        }
                    };

                    Button btnUsar = new Button
                    {
                        Text = esClaseMagica ? "LANZAR" : "USAR",
                        Location = new Point(200, 500),
                        Size = new Size(250, 50),
                        BackColor = esClaseMagica ? Color.DarkBlue : Color.DarkGreen,
                        ForeColor = Color.White,
                        Font = new Font("Papyrus", 12, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat
                    };
                    btnUsar.FlatAppearance.BorderColor = Color.Gold;
                    btnUsar.FlatAppearance.BorderSize = 2;

                    btnUsar.Click += (s, ev) =>
                    {
                        if (lista.SelectedIndex >= 0)
                        {
                            var hab = habilidades[lista.SelectedIndex];
                            formHabilidades.Close();
                            UsarHabilidadOHechizo(hab.Nombre, hab.Dano, hab.EsHechizo);
                        }
                        else
                        {
                            MessageBox.Show("Selecciona primero", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    };

                    formHabilidades.Controls.Add(lblTitulo);
                    formHabilidades.Controls.Add(lista);
                    formHabilidades.Controls.Add(lblDesc);
                    formHabilidades.Controls.Add(btnUsar);
                    formHabilidades.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ObtenerDanoHabilidad(int idHabilidad)
        {
            Dictionary<int, int> danos = new Dictionary<int, int>
            {
                {1, 30},  {2, 20},  {3, 28},  {4, 0},    // Guerrero
                {5, 40},  {6, 35},  {7, 55},  {8, -40},  // Mago
                {9, 32},  {10, 38}, {11, 0},  {12, -30}, // Paladín
                {13, 35}, {14, -40},{15, 33}, {16, 0}    // Clérigo
            };
            return danos.ContainsKey(idHabilidad) ? danos[idHabilidad] : 0;
        }

        private async void UsarHabilidadOHechizo(string nombre, int efecto, bool esHechizo)
        {
            string tipoAccion = esHechizo ? "✨ HECHIZO" : "⚔️ HABILIDAD";

            if (efecto > 0) // DAÑO
            {
                enemigoActual.HP -= efecto;
                danoTotalInfligido += efecto;

                lblEstadoCombate.Text = $"{tipoAccion}: {nombre}\n\n" +
                    $"{jugador.Nombre} causa {efecto} de daño\n\n" +
                    $"HP Enemigo: {enemigoActual.HP}/{enemigoActual.HPMaximo}";
                lblEstadoCombate.ForeColor = esHechizo ? Color.MediumPurple : Color.Gold;

                ActualizarUIEnemigo();
            }
            else if (efecto < 0) // CURACIÓN
            {
                int curacion = -efecto;
                jugador.HP += curacion;
                if (jugador.HP > jugador.HPMaximo) jugador.HP = jugador.HPMaximo;

                lblEstadoCombate.Text = $"{tipoAccion}: {nombre}\n\n" +
                    $"{jugador.Nombre} se cura {curacion} HP\n\n" +
                    $"HP Actual: {jugador.HP}/{jugador.HPMaximo}";
                lblEstadoCombate.ForeColor = Color.LightGreen;

                ActualizarUIJugador();
            }
            else // BUFF/DEFENSA
            {
                lblEstadoCombate.Text = $"{tipoAccion}: {nombre}\n\n" +
                    $"{jugador.Nombre} se fortalece\n" +
                    $"Bonus aplicado temporalmente";
                lblEstadoCombate.ForeColor = Color.Cyan;

                jugador.Constitucion += 5;
            }

            System.Threading.Thread.Sleep(2000);

            if (!VerificarFinCombate())
            {
                if (efecto == 0)
                {
                    await TurnoEnemigo();
                    jugador.Constitucion -= 5;
                }
                else
                {
                    await TurnoEnemigo();
                }
            }
        }

        private async void BtnDefender_Click(object sender, EventArgs e)
        {
            if (!turnoJugador) return;

            lblEstadoCombate.Text = $"🛡️ POSICIÓN DEFENSIVA 🛡️\n\n" +
                $"{jugador.Nombre} se prepara para el ataque\n\n" +
                $"Bonus: +5 Constitución\n" +
                $"Este turno recibirás menos daño";
            lblEstadoCombate.ForeColor = Color.Cyan;

            jugador.Constitucion += 5;

            System.Threading.Thread.Sleep(1500);
            await TurnoEnemigo();

            jugador.Constitucion -= 5;
        }

        private void BtnInventario_Click(object sender, EventArgs e)
        {
            if (!turnoJugador) return;

            int hpAntes = jugador.HP;
            Form10 inv = new Form10(usuarioId);
            inv.ShowDialog();

            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();
                    using (MySqlCommand cmd = new MySqlCommand("CALL ObtenerHPActual(@p_jugador_id)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@p_jugador_id", usuarioId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                jugador.HP = reader.GetInt32("HP");
                        }
                    }
                }

                int curado = jugador.HP - hpAntes;
                if (curado > 0)
                {
                    AgregarMensaje($"🧪 Poción usada", Color.Green);
                    AgregarMensaje($"   💚 Curado: +{curado} HP", Color.LightGreen);
                    AgregarMensaje($"   HP: {jugador.HP}/{jugador.HPMaximo}\n", Color.Green);
                    ActualizarUIJugador();

                    System.Threading.Tasks.Task.Run(async () =>
                    {
                        await System.Threading.Tasks.Task.Delay(1500);
                        await TurnoEnemigo();
                    });
                }
                else
                {
                    AgregarMensaje($"📦 Inventario cerrado sin cambios\n", Color.Gray);
                }
            }
            catch (Exception ex)
            {
                AgregarMensaje($"❌ Error: {ex.Message}", Color.Red);
            }
        }

        private async void BtnHuir_Click(object sender, EventArgs e)
        {
            int dado = TirarDado(20);
            int total = dado + jugador.Destreza;

            if (total >= 15)
            {
                lblEstadoCombate.Text = $"💨 HUIDA EXITOSA 💨\n\n" +
                    $"D20: {dado} + DES: {jugador.Destreza} = {total}\n\n" +
                    $"¡Logras escapar del combate!";
                lblEstadoCombate.ForeColor = Color.Yellow;

                System.Threading.Thread.Sleep(1500);
                GuardarProgreso();
                this.Close();
                formularioOrigen?.Show();
            }
            else
            {
                lblEstadoCombate.Text = $"❌ NO PUEDES HUIR ❌\n\n" +
                    $"D20: {dado} + DES: {jugador.Destreza} = {total}\n" +
                    $"Necesitas 15 o más\n\n" +
                    $"¡{enemigoActual.Nombre} te bloquea la salida!";
                lblEstadoCombate.ForeColor = Color.Red;

                System.Threading.Thread.Sleep(1500);
                await TurnoEnemigo();
            }
        }

        private async System.Threading.Tasks.Task TurnoEnemigo()
        {
            turnoJugador = false;
            BloquearBotones();

            System.Threading.Thread.Sleep(800);

            int dado = TirarDado(20);
            int ataqueEnemigo = dado + enemigoActual.Fuerza;
            int defensaJugador = 10 + jugador.Destreza;

            if (ataqueEnemigo >= defensaJugador)
            {
                int dadoDano = TirarDado(8);
                int dano = Math.Max(1, dadoDano + enemigoActual.Fuerza - jugador.Constitucion);
                jugador.HP -= dano;
                danoTotalRecibido += dano;

                lblEstadoCombate.Text = $"💀 ATAQUE ENEMIGO 💀\n\n" +
                    $"{enemigoActual.Nombre} te golpea\n" +
                    $"D20: {dado}\n" +
                    $"Daño: {dano} HP\n\n" +
                    $"Tu HP: {jugador.HP}/{jugador.HPMaximo}";
                lblEstadoCombate.ForeColor = Color.OrangeRed;

                ActualizarUIJugador();
            }
            else
            {
                lblEstadoCombate.Text = $"🎯 ESQUIVASTE 🎯\n\n" +
                    $"{enemigoActual.Nombre} intenta atacar\n" +
                    $"D20: {dado}\n\n" +
                    $"¡Logras esquivar el ataque!";
                lblEstadoCombate.ForeColor = Color.Cyan;
            }

            System.Threading.Thread.Sleep(2000);

            if (!VerificarFinCombate())
            {
                turnoNumero++;
                lblTurno.Text = $"Turno {turnoNumero}";
                turnoJugador = true;
                DesbloquearBotones();

                lblEstadoCombate.Text = $"⚔️ TU TURNO ⚔️\n\n" +
                    $"Elige tu acción";
                lblEstadoCombate.ForeColor = Color.White;
            }
        }

        private bool VerificarFinCombate()
        {
            if (enemigoActual.HP <= 0)
            {
                this.Invoke(new Action(() =>
                {
                    AgregarMensaje("═══════════════════════════════════", Color.Gold);
                    AgregarMensaje($"🏆 ¡VICTORIA! 🏆", Color.Yellow);
                    AgregarMensaje($"Has derrotado a {enemigoActual.Nombre}", Color.White);
                    AgregarMensaje($"Turnos: {turnoNumero} | Daño: {danoTotalInfligido}", Color.Gray);
                    AgregarMensaje("═══════════════════════════════════\n", Color.Gold);

                    OtorgarRecompensas();
                    GuardarLogCombateMongoDB("victoria", turnoNumero);

                    System.Threading.Tasks.Task.Delay(2000).ContinueWith(_ =>
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }));
                    });
                }));
                return true;
            }
            else if (jugador.HP <= 0)
            {
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                        {
                            conexion.Open();

                            // Usar CALL para actualizar muerte
                            using (MySqlCommand cmd = new MySqlCommand("CALL ActualizarMetricasMuerte(@p_jugador_id)", conexion))
                            {
                                cmd.Parameters.AddWithValue("@p_jugador_id", usuarioId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch { }

                    AgregarMensaje("═══════════════════════════════════", Color.DarkRed);
                    AgregarMensaje($"💀 DERROTA 💀", Color.Red);
                    AgregarMensaje($"Has sido derrotado por {enemigoActual.Nombre}", Color.White);
                    AgregarMensaje($"Turnos sobrevividos: {turnoNumero}", Color.Gray);
                    AgregarMensaje("═══════════════════════════════════\n", Color.DarkRed);

                    jugador.HP = 1;
                    GuardarProgreso();
                    GuardarLogCombateMongoDB("derrota", turnoNumero);

                    System.Threading.Tasks.Task.Delay(2000).ContinueWith(_ =>
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.Close();
                            formularioOrigen?.Show();
                        }));
                    });
                }));
                return true;
            }
            return false;
        }


        private void OtorgarRecompensas()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();

                    using (MySqlCommand cmd = new MySqlCommand("CALL OtorgarRecompensasCombate(@JugadorId, @NivelEnemigo, @NombreEnemigo, @Turnos, @DanoInfligido, @DanoRecibido)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@JugadorId", usuarioId);
                        cmd.Parameters.AddWithValue("@NivelEnemigo", enemigoActual.Nivel);
                        cmd.Parameters.AddWithValue("@NombreEnemigo", enemigoActual.Nombre);
                        cmd.Parameters.AddWithValue("@Turnos", turnoNumero);
                        cmd.Parameters.AddWithValue("@DanoInfligido", danoTotalInfligido);
                        cmd.Parameters.AddWithValue("@DanoRecibido", danoTotalRecibido);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int exp = reader.GetInt32("EXPGanada");
                                int oro = reader.GetInt32("OroGanado");
                                bool subio = reader.GetBoolean("SubioDeNivel");

                                AgregarMensaje($"💰 Recompensas:", Color.Gold);
                                AgregarMensaje($"   +{exp} EXP", Color.Cyan);
                                AgregarMensaje($"   +{oro} Oro", Color.Yellow);

                                if (subio)
                                {
                                    int nivelNuevo = reader.GetInt32("NivelNuevo");
                                    AgregarMensaje($"   ⭐ ¡SUBISTE AL NIVEL {nivelNuevo}! ⭐", Color.Gold);
                                }
                                AgregarMensaje("", Color.White);

                                // Guardar para MongoDB
                                expGanada = exp;
                                oroGanado = oro;
                                subioNivel = subio;
                            }
                        }
                    }

                    // Actualizar métricas usando CALL
                    using (MySqlCommand cmd = new MySqlCommand("CALL ActualizarMetricasCombate(@p_jugador_id, @p_enemigo_nombre)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@p_jugador_id", usuarioId);
                        cmd.Parameters.AddWithValue("@p_enemigo_nombre", enemigoActual.Nombre);
                        cmd.ExecuteNonQuery();
                    }

                    // Verificar logros
                    using (MySqlCommand cmdLogros = new MySqlCommand("CALL VerificarLogros(@id)", conexion))
                    {
                        cmdLogros.Parameters.AddWithValue("@id", usuarioId);
                        cmdLogros.ExecuteNonQuery();
                    }

                    GuardarProgreso();
                }
            }
            catch (Exception ex)
            {
                AgregarMensaje($"❌ Error recompensas: {ex.Message}", Color.Red);
            }
        }
        private void AgregarMensaje(string texto, Color color)
        {
            // Usar el label de estado
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AgregarMensaje(texto, color)));
                return;
            }

            // Acumular mensajes en el label
            if (lblEstadoCombate.Text.Length > 500) // Limpiar si es muy largo
                lblEstadoCombate.Text = "";

            lblEstadoCombate.Text += texto + "\n";
            lblEstadoCombate.ForeColor = color;
        }
        private void GuardarLogCombateMongoDB(string resultado, int turnoFinal)
        {
            try
            {
                if (string.IsNullOrEmpty(Form9.SesionIdActual))
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No hay sesión activa para guardar el combate");
                    return;
                }

                // Obtener nombre y nivel del jugador
                string nombreJugador = jugador.Nombre;
                int nivelJugador = jugador.Nivel;

                // Crear el log de combate
                var combatLog = new Database.MongoDB.Models.CombatLog
                {
                    JugadorId = Form9.JugadorIdActual,
                    JugadorNombre = nombreJugador,
                    SesionId = Form9.SesionIdActual,
                    EnemigoId = enemigoActual.ID,
                    EnemigoNombre = enemigoActual.Nombre,
                    NivelJugador = nivelJugador,
                    NivelEnemigo = enemigoActual.Nivel,
                    Resultado = resultado,
                    DuracionTurnos = turnoFinal,
                    HPInicialJugador = jugador.HPMaximo,
                    HPFinalJugador = jugador.HP,
                    HPInicialEnemigo = enemigoActual.HPMaximo,
                    HPFinalEnemigo = enemigoActual.HP,
                    DanoTotalJugador = danoTotalInfligido,
                    DanoTotalEnemigo = danoTotalRecibido,
                    CriticosJugador = criticosJugador,
                    ExpGanada = expGanada,
                    OroGanado = oroGanado,
                    SubioNivel = subioNivel,
                    Ubicacion = "Mapa Principal" // Puedes cambiar esto según donde esté el jugador
                };

                // Guardar en MongoDB
                var logService = new Database.MongoDB.Services.CombatLogService();
                logService.GuardarCombate(combatLog);

                // Registrar en la sesión
                var sessionService = new Database.MongoDB.Services.SessionService();
                sessionService.RegistrarCombate(Form9.SesionIdActual, resultado == "victoria");

                if (resultado == "victoria")
                {
                    sessionService.RegistrarExperiencia(Form9.SesionIdActual, expGanada);
                    sessionService.RegistrarTransaccionOro(Form9.SesionIdActual, oroGanado, true);
                }

                System.Diagnostics.Debug.WriteLine($"✅ Combate guardado en MongoDB - Resultado: {resultado}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Error guardando log de combate: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void GuardarProgreso()
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection("Server=localhost;Database=proyecto;Uid=root;Pwd=;"))
                {
                    conexion.Open();
                    using (MySqlCommand cmd = new MySqlCommand("CALL ActualizarHPJugador(@p_jugador_id, @p_nuevo_hp)", conexion))
                    {
                        cmd.Parameters.AddWithValue("@p_jugador_id", usuarioId);
                        cmd.Parameters.AddWithValue("@p_nuevo_hp", jugador.HP);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        private int TirarDado(int caras)
        {
            Random rnd = new Random();
            System.Threading.Thread.Sleep(10);
            return rnd.Next(1, caras + 1);
        }

        private void BloquearBotones()
        {
            foreach (var btn in botonesAccion)
                btn.Enabled = false;
        }

        private void DesbloquearBotones()
        {
            foreach (var btn in botonesAccion)
                btn.Enabled = true;
        }

        private void ActualizarUIJugador()
        {
            lblHPJugador.Text = $"HP: {jugador.HP} / {jugador.HPMaximo}";

            float porcentaje = (float)jugador.HP / jugador.HPMaximo;
            if (porcentaje > 0.6f)
                lblHPJugador.ForeColor = Color.LimeGreen;
            else if (porcentaje > 0.3f)
                lblHPJugador.ForeColor = Color.Yellow;
            else
                lblHPJugador.ForeColor = Color.Red;
        }

        private void ActualizarUIEnemigo()
        {
            lblHPEnemigo.Text = $"HP: {enemigoActual.HP} / {enemigoActual.HPMaximo}";

            float porcentaje = (float)enemigoActual.HP / enemigoActual.HPMaximo;
            if (porcentaje > 0.6f)
                lblHPEnemigo.ForeColor = Color.Red;
            else if (porcentaje > 0.3f)
                lblHPEnemigo.ForeColor = Color.Orange;
            else
                lblHPEnemigo.ForeColor = Color.DarkRed;
        }

        private Image CargarImagenDesdeRecursos(string nombreImagen)
        {
            var ensamblado = Assembly.GetExecutingAssembly();
            var rutaCompleta = $"Proyecto_Dnd.Recursos.{nombreImagen}";

            try
            {
                using (Stream stream = ensamblado.GetManifestResourceStream(rutaCompleta))
                {
                    if (stream != null)
                        return Image.FromStream(stream);
                }
            }
            catch { }
            return null;
        }

        public class Jugador
        {
            public string Nombre { get; set; }
            public int HP { get; set; }
            public int HPMaximo { get; set; }
            public int Fuerza { get; set; }
            public int Destreza { get; set; }
            public int Constitucion { get; set; }
            public int Inteligencia { get; set; }
            public int ClaseId { get; set; }
            public int Nivel { get; set; }
        }

        public class Enemigo
        {
            public int ID { get; set; }
            public string Nombre { get; set; }
            public int HP { get; set; }
            public int HPMaximo { get; set; }
            public int Fuerza { get; set; }
            public int Destreza { get; set; }
            public int Constitucion { get; set; }
            public int Inteligencia { get; set; }
            public int Nivel { get; set; }
        }
    }
}