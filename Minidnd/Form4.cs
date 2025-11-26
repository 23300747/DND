using System;
using System.Media;
using System.Windows.Forms;
using System.Drawing;

namespace Proyecto_Dnd
{
    public partial class Form4 : Form
    {
        SoundPlayer narrador;
        Button btnOmitir;

        public Form4()
        {
            InitializeComponent();
            MostrarFondo();
            CrearBotonOmitir();
            ReproducirNarracion();
        }

        private void MostrarFondo()
        {
            this.BackgroundImage = Image.FromFile("fondo.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void CrearBotonOmitir()
        {
            btnOmitir = new Button();
            btnOmitir.Text = "Omitir";
            btnOmitir.Font = new Font("Arial", 12, FontStyle.Bold);
            btnOmitir.BackColor = Color.DarkRed;
            btnOmitir.ForeColor = Color.White;
            btnOmitir.Size = new Size(100, 40);
            btnOmitir.Location = new Point(this.ClientSize.Width - 120, this.ClientSize.Height - 60);
            btnOmitir.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOmitir.Click += BtnOmitir_Click;
            this.Controls.Add(btnOmitir);
        }

        private void ReproducirNarracion()
        {
            narrador = new SoundPlayer("Historia.wav");
            narrador.Play(); // se reproduce en segundo plano, no bloquea la UI
        }

        private void BtnOmitir_Click(object sender, EventArgs e)
        {
            if (narrador != null)
            {
                narrador.Stop(); // detener narración
            }

            AbrirForm5();
        }

        private void AbrirForm5()
        {
            Form5 Mp1 = new Form5();
            Mp1.Show();
            this.Hide();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (narrador != null)
            {
                narrador.Stop();
            }
            base.OnFormClosing(e);
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }
    }
}