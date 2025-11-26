using System;
using System.Drawing;
using System.Windows.Forms;

namespace Proyecto_Dnd
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            ConfigurarMenu();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void ConfigurarMenu()
        {
            this.BackgroundImage = Image.FromFile("dragon.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Text = "Dungeons && Dragons - Isla de las Tempestades";

            int margenDerecho = 50;
            int anchoBoton = 300;

            Label lblTitulo = new Label
            {
                Text = "DRAGONES DE LA ISLA\nDE LAS TEMPESTADES",
                Font = new Font("Papyrus", 13, FontStyle.Bold),
                ForeColor = Color.Gold,
                BackColor = Color.FromArgb(100, 0, 0, 0),
                AutoSize = false,
                Size = new Size(500, 100),
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 30
            };
            lblTitulo.Left = this.ClientSize.Width - lblTitulo.Width - margenDerecho;
            lblTitulo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.Controls.Add(lblTitulo);

            Button btnNuevaPartida = CrearBoton("NUEVA PARTIDA", 160);
            btnNuevaPartida.Click += (s, e) =>
            {
                Form2 creacion = new Form2();
                creacion.Show();
                this.Hide();
            };

            Button btnContinuar = CrearBoton("CONTINUAR o Iniciar partida", 210);
            btnContinuar.Click += (s, e) =>
            {
                Form9 login = new Form9();
                login.Show();
                this.Hide();
            };

            Button btnCreditos = CrearBoton("CRÉDITOS", 260);
            btnCreditos.Click += (s, e) =>
            {
                Form14 creditos = new Form14();
                creditos.ShowDialog();
            };

            Button btnSalir = CrearBoton("SALIR", 310);
            btnSalir.Click += (s, e) => Application.Exit();

            this.Controls.Add(btnNuevaPartida);
            this.Controls.Add(btnContinuar);
            this.Controls.Add(btnCreditos);
            this.Controls.Add(btnSalir);

            Label lblVersion = new Label
            {
                Text = "v22.0 - Proyecto Académico 2025",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(10, this.ClientSize.Height - 30)
            };
            lblVersion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(lblVersion);
        }

        private Button CrearBoton(string texto, int topOffset)
        {
            int margenDerecho = 50;
            int anchoBoton = 300;

            Button btn = new Button
            {
                Text = texto,
                Font = new Font("Papyrus", 12, FontStyle.Bold),
                Size = new Size(anchoBoton, 45),
                BackColor = Color.FromArgb(139, 69, 19),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Top = topOffset,
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderColor = Color.Gold;
            btn.FlatAppearance.BorderSize = 2;
            btn.Left = this.ClientSize.Width - btn.Width - margenDerecho;
            btn.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(160, 82, 45);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(139, 69, 19);

            return btn;
        }
    }
}