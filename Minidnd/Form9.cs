using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Proyecto_Dnd
{
    public partial class Form9 : Form
    {
        // Propiedades estáticas para mantener el estado global
        public static int JugadorIdActual { get; private set; }
        public static string SesionIdActual { get; private set; }

        public Form9()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Dungeons & Dragons - Login";
            this.Size = new Size(450, 320);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(40, 30, 20);

            Panel panelPrincipal = new Panel
            {
                Size = new Size(400, 260),
                Location = new Point(25, 20),
                BackColor = Color.FromArgb(60, 45, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label lblTitulo = new Label
            {
                Text = "ACCESO A PARTIDA",
                Font = new Font("Papyrus", 16, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = false,
                Size = new Size(360, 40),
                Location = new Point(20, 15),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblUsuario = new Label
            {
                Text = "Nombre de Jugador:",
                Font = new Font("Papyrus", 10),
                ForeColor = Color.White,
                Location = new Point(30, 70)
            };

            TextBox txtUsuario = new TextBox
            {
                Name = "txtUsuario",
                Location = new Point(30, 95),
                Width = 340,
                Height = 30,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(240, 235, 220)
            };

            Label lblContrasena = new Label
            {
                Text = "Contraseña:",
                Font = new Font("Papyrus", 10),
                ForeColor = Color.White,
                Location = new Point(30, 135)
            };

            TextBox txtContrasena = new TextBox
            {
                Name = "txtContrasena",
                Location = new Point(30, 160),
                Width = 340,
                Height = 30,
                Font = new Font("Segoe UI", 11),
                PasswordChar = '●',
                BackColor = Color.FromArgb(240, 235, 220)
            };

            Button btnAcceder = new Button
            {
                Text = "CONTINUAR AVENTURA",
                Location = new Point(80, 205),
                Width = 240,
                Height = 40,
                Font = new Font("Papyrus", 11, FontStyle.Bold),
                BackColor = Color.SaddleBrown,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAcceder.FlatAppearance.BorderColor = Color.Gold;
            btnAcceder.FlatAppearance.BorderSize = 2;

            btnAcceder.Click += (s, e) =>
            {
                string nombre = txtUsuario.Text.Trim();
                string contrasena = txtContrasena.Text;

                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(contrasena))
                {
                    MessageBox.Show("Por favor, completa todos los campos.",
                        "Campos vacíos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                int jugadorId = ValidarCredenciales(nombre, contrasena);

                if (jugadorId > 0)
                {
                    JugadorIdActual = jugadorId;
                    MessageBox.Show($"¡Bienvenido de vuelta, {nombre}!\nID de jugador: {jugadorId}",
                        "Acceso concedido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    Form4 partida = new Form4();
                    partida.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.",
                        "Error de acceso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            };

            panelPrincipal.Controls.Add(lblTitulo);
            panelPrincipal.Controls.Add(lblUsuario);
            panelPrincipal.Controls.Add(txtUsuario);
            panelPrincipal.Controls.Add(lblContrasena);
            panelPrincipal.Controls.Add(txtContrasena);
            panelPrincipal.Controls.Add(btnAcceder);
            this.Controls.Add(panelPrincipal);
        }

        private void Form9_Load(object sender, EventArgs e)
        {
        }

        private int ValidarCredenciales(string nombre, string contrasena)
        {
            string cadenaConexion = "Server=localhost;Database=proyecto;Uid=root;Pwd=;";

            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();
                    using (MySqlCommand comando = new MySqlCommand("ValidarCredenciales", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.AddWithValue("nombreJugador", nombre);
                        comando.Parameters.AddWithValue("contrasenaJugador", contrasena);

                        object resultado = comando.ExecuteScalar();

                        if (resultado != null && resultado != DBNull.Value)
                        {
                            int idObtenido = Convert.ToInt32(resultado);
                            System.Diagnostics.Debug.WriteLine($"Login exitoso - Usuario: {nombre}, ID: {idObtenido}");

                            try
                            {
                                using (MySqlCommand cmdDatos = new MySqlCommand("ObtenerDatosJugador", conexion))
                                {
                                    cmdDatos.CommandType = CommandType.StoredProcedure;
                                    cmdDatos.Parameters.AddWithValue("pJugadorId", idObtenido);

                                    using (MySqlDataReader reader = cmdDatos.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            var sessionService = new Database.MongoDB.Services.SessionService();

                                            SesionIdActual = sessionService.IniciarSesion(
                                                jugadorId: idObtenido,
                                                nombreJugador: reader.GetString("Nombre"),
                                                nivel: reader.GetInt32("ID_Nivel"),
                                                hp: reader.GetInt32("HP"),
                                                exp: reader.GetInt32("EXP"),
                                                oro: reader.GetInt32("Oro"),
                                                clase: reader.IsDBNull(reader.GetOrdinal("Clase"))
                                                    ? "Sin Clase"
                                                    : reader.GetString("Clase"),
                                                ubicacionInicial: "Menú Principal"
                                            );

                                            System.Diagnostics.Debug.WriteLine($"✅ Sesión MongoDB iniciada: {SesionIdActual}");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            { }

                            return idObtenido;
                        }

                        System.Diagnostics.Debug.WriteLine($"Login fallido - Usuario: {nombre}");
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return 0;
            }
        }

        // Método opcional para cerrar sesión cuando se cierre el formulario
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (!string.IsNullOrEmpty(SesionIdActual))
            {
                try
                {
                    var sessionService = new Database.MongoDB.Services.SessionService();
                    sessionService.FinalizarSesion(SesionIdActual);
                    System.Diagnostics.Debug.WriteLine($"✅ Sesión cerrada: {SesionIdActual}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Error cerrando sesión: {ex.Message}");
                }
            }
        }
    }
}