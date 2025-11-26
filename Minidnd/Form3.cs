using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Proyecto_Dnd
{
    public partial class Form3 : Form
    {
        string clase;
        int[] STS;
        int idClase = 0;

        Dictionary<string, int> alineamientos = new Dictionary<string, int>()
        {
            { "Legal Bueno", 1 }, { "Neutral Bueno", 2 }, { "Caótico Bueno", 3 },
            { "Legal Neutral", 4 }, { "Neutral Puro", 5 }, { "Caótico Neutral", 6 },
            { "Legal Malvado", 7 }, { "Neutral Malvado", 8 }, { "Caótico Malvado", 9 }
        };

        Dictionary<string, int> subrazas = new Dictionary<string, int>()
        {
            { "Alto Elfo", 1 }, { "Elfo Oscuro", 2 }, { "Enano de las Montañas", 3 },
            { "Enano de las Colinas", 4 }, { "Humano", 5 }, { "Mediano", 6 },
            { "Semielfo", 7 }, { "Semiorco", 8 }, { "Tiefling", 9 }
        };

        Dictionary<string, int> transfondos = new Dictionary<string, int>()
        {
            { "Erudito", 1 }, { "Soldado", 2 }, { "Acolito", 3 }, { "Criminal", 4 },
            { "Noble", 5 }, { "Cazador", 6 }, { "Exiliado", 7 }, { "Profanador", 8 }
        };

        public Form3(string claseSeleccionada, int[] STS3)
        {
            InitializeComponent();
            clase = claseSeleccionada;
            if (clase == "Guerrero") idClase = 1;
            else if (clase == "Mago") idClase = 2;
            else if (clase == "Paladín") idClase = 3;
            else if (clase == "Clérigo") idClase = 4;
            STS = STS3;
            Resumen();
        }

        private void Form3_Load(object sender, EventArgs e) { }

        private void Resumen()
        {
            this.BackColor = Color.FromArgb(210, 180, 140);
            int baseY = 20;

            Label Res = new Label();
            Res.Font = new Font("Papyrus", 11, FontStyle.Bold);
            Res.ForeColor = Color.White;
            Res.BackColor = Color.FromArgb(180, 0, 0, 0);
            Res.AutoSize = true;
            Res.Location = new Point(20, 20);
            string[] nombres = { "HP", "F", "D", "C", "I", "S", "Ca", "In" };
            string texto = $"Clase: {clase}\n\nSTS finales:\n";
            for (int i = 0; i < STS.Length; i++)
                texto += $"{nombres[i]}: {STS[i]}\n";
            Res.Text = texto;
            this.Controls.Add(Res);

            Label Nombre = new Label();
            Nombre.Text = "Nombre del personaje:";
            Nombre.ForeColor = Color.OrangeRed;
            Nombre.Font = new Font("Papyrus", 15, FontStyle.Regular);
            Nombre.Location = new Point(200, baseY + 30);
            this.Controls.Add(Nombre);

            TextBox Nom = new TextBox();
            Nom.Name = "Nom";
            Nom.Font = new Font("Papyrus", 15);
            Nom.Size = new Size(200, 30);
            Nom.Location = new Point(180, baseY + 75);
            this.Controls.Add(Nom);

            Label Tital = new Label();
            Tital.Text = "Alineamiento:";
            Tital.AutoSize = true;
            Tital.ForeColor = Color.OrangeRed;
            Tital.Font = new Font("Papyrus", 15, FontStyle.Regular);
            Tital.Location = new Point(200, baseY + 130);
            this.Controls.Add(Tital);

            ComboBox Alineamiento = new ComboBox();
            Alineamiento.Name = "cmbAlineamiento";
            Alineamiento.Font = new Font("Papyrus", 11);
            Alineamiento.DropDownStyle = ComboBoxStyle.DropDownList;
            Alineamiento.Location = new Point(180, baseY + 170);
            Alineamiento.Size = new Size(200, 30);
            Alineamiento.Items.AddRange(new string[]
            {
                "Legal Bueno", "Neutral Bueno", "Caótico Bueno",
                "Legal Neutral", "Neutral Puro", "Caótico Neutral",
                "Legal Malvado", "Neutral Malvado", "Caótico Malvado"
            });
            this.Controls.Add(Alineamiento);

            Label Titta = new Label();
            Titta.Text = "Transfondo:";
            Titta.ForeColor = Color.OrangeRed;
            Titta.AutoSize = true;
            Titta.Font = new Font("Papyrus", 15, FontStyle.Regular);
            Titta.Location = new Point(20, baseY + 270);
            this.Controls.Add(Titta);

            ComboBox transfondo = new ComboBox();
            transfondo.Name = "transfondo";
            transfondo.DropDownStyle = ComboBoxStyle.DropDownList;
            transfondo.Font = new Font("Papyrus", 11, FontStyle.Regular);
            transfondo.Size = new Size(200, 30);
            transfondo.Location = new Point(20, baseY + 300);
            transfondo.Items.AddRange(new string[]
            {
                "Erudito", "Soldado", "Acolito", "Criminal",
                "Noble", "Cazador", "Exiliado", "Profanador"
            });
            this.Controls.Add(transfondo);

            Label Titsub = new Label();
            Titsub.Text = "Subraza:";
            Titsub.ForeColor = Color.OrangeRed;
            Titsub.Font = new Font("Papyrus", 15, FontStyle.Regular);
            Titsub.Location = new Point(20, baseY + 370);
            this.Controls.Add(Titsub);

            ComboBox Subraza = new ComboBox();
            Subraza.Name = "cmbSubraza";
            Subraza.Font = new Font("Papyrus", 11);
            Subraza.DropDownStyle = ComboBoxStyle.DropDownList;
            Subraza.Location = new Point(20, baseY + 400);
            Subraza.Size = new Size(200, 30);
            Subraza.Items.AddRange(new string[]
            {
                "Alto Elfo", "Elfo Oscuro", "Enano de las Montañas",
                "Enano de las Colinas", "Humano", "Mediano",
                "Semielfo", "Semiorco", "Tiefling"
            });
            this.Controls.Add(Subraza);

            Button Guardar = new Button();
            Guardar.Text = "Guardar personaje";
            Guardar.Font = new Font("Papyrus", 11, FontStyle.Bold);
            Guardar.Size = new Size(180, 40);
            Guardar.Location = new Point(400, 400);
            Guardar.BackColor = Color.SaddleBrown;
            Guardar.ForeColor = Color.White;
            Guardar.FlatStyle = FlatStyle.Flat;
            Guardar.FlatAppearance.BorderSize = 0;
            Guardar.Click += Aceptar;
            this.Controls.Add(Guardar);

            Label Titcon = new Label();
            Titcon.Text = "Contraseña:";
            Titcon.ForeColor = Color.OrangeRed;
            Titcon.Font = new Font("Papyrus", 15, FontStyle.Regular);
            Titcon.AutoSize = true;
            Titcon.Location = new Point(400, baseY + 30);
            this.Controls.Add(Titcon);

            TextBox Contrasena = new TextBox();
            Contrasena.Name = "Contrasena";
            Contrasena.Font = new Font("Papyrus", 11);
            Contrasena.Size = new Size(200, 30);
            Contrasena.Location = new Point(400, baseY + 70);
            Contrasena.PasswordChar = '•';
            this.Controls.Add(Contrasena);
        }

        private void Aceptar(object sender, EventArgs e)
        {
            TextBox Nom = Controls.Find("Nom", true).FirstOrDefault() as TextBox;
            TextBox Contrasena = Controls.Find("Contrasena", true).FirstOrDefault() as TextBox;
            ComboBox Alineamiento = Controls.Find("cmbAlineamiento", true).FirstOrDefault() as ComboBox;
            ComboBox Subraza = Controls.Find("cmbSubraza", true).FirstOrDefault() as ComboBox;
            ComboBox transfondo = Controls.Find("transfondo", true).FirstOrDefault() as ComboBox;

            if (Nom == null || Contrasena == null || Alineamiento == null || Subraza == null || transfondo == null)
            {
                MessageBox.Show("Error interno: no se encontraron todos los controles.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(Nom.Text) ||
                string.IsNullOrWhiteSpace(Contrasena.Text) ||
                Alineamiento.SelectedItem == null ||
                Subraza.SelectedItem == null ||
                transfondo.SelectedItem == null)
            {
                MessageBox.Show("Por favor, completa todos los campos antes de guardar.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string alineamientoSeleccionado = Alineamiento.SelectedItem.ToString();
            string subrazaSeleccionada = Subraza.SelectedItem.ToString();
            string transfondoSeleccionado = transfondo.SelectedItem.ToString();

            if (!alineamientos.TryGetValue(alineamientoSeleccionado, out int idAlineamiento) ||
                !subrazas.TryGetValue(subrazaSeleccionada, out int idSubraza) ||
                !transfondos.TryGetValue(transfondoSeleccionado, out int idTransfondo))
            {
                MessageBox.Show("Error al convertir alineamiento, subraza o transfondo a ID.", "Error de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string cadenaConexion = "Server=localhost;Database=proyecto;Uid=root;Pwd=;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(cadenaConexion))
                {
                    conn.Open();

                    string queryVerificar = "SELECT COUNT(*) FROM jugador WHERE Contrasena = @pass";
                    using (MySqlCommand cmdVerificar = new MySqlCommand(queryVerificar, conn))
                    {
                        cmdVerificar.Parameters.AddWithValue("@pass", Contrasena.Text);
                        int count = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show(
                                "⚠ Esta contraseña ya está en uso.\n\n" +
                                "La contraseña debe ser ÚNICA para cada jugador.\n" +
                                "(Puedes usar el mismo nombre, pero diferente contraseña)\n\n" +
                                "Por favor, elige otra contraseña.",
                                "Contraseña duplicada",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            Contrasena.Clear();
                            Contrasena.Focus();
                            return;
                        }
                    }

                    using (MySqlCommand cmd = new MySqlCommand("GuardarPersonaje", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("Nombre", Nom.Text);
                        cmd.Parameters.AddWithValue("Contrasena", Contrasena.Text);
                        cmd.Parameters.AddWithValue("ID_Clase", idClase);
                        cmd.Parameters.AddWithValue("ID_Alineamiento", idAlineamiento);
                        cmd.Parameters.AddWithValue("ID_Subraza", idSubraza);
                        cmd.Parameters.AddWithValue("ID_Transfondo", idTransfondo);
                        cmd.Parameters.AddWithValue("HP", STS[0]);
                        cmd.Parameters.AddWithValue("Fuerza", STS[1]);
                        cmd.Parameters.AddWithValue("Sabiduria", STS[5]);
                        cmd.Parameters.AddWithValue("Inteligencia", STS[4]);
                        cmd.Parameters.AddWithValue("Constitucion", STS[3]);
                        cmd.Parameters.AddWithValue("Destreza", STS[2]);
                        cmd.Parameters.AddWithValue("Carisma", STS[6]);
                        cmd.Parameters.AddWithValue("Iniciativa", STS[7]);

                        cmd.ExecuteNonQuery();
                    }
                }

                string habilidadesClase = idClase switch
                {
                    1 => "Golpe Demoledor, Tajo Giratorio",
                    2 => "Proyectil Mágico",
                    3 => "Golpe Divino, Escudo de Fe",
                    4 => "Curar Heridas, Luz Sagrada",
                    _ => "Habilidades básicas"
                };

                
                // Ir a la siguiente pantalla
                Form1 aventura = new Form1();
                aventura.Show();
                this.Close();
            }
            catch { }
        }
    }
}