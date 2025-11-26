using System.Reflection;
namespace Proyecto_Dnd
{
    public partial class Form2 : Form
    {
        Panel Eleccion;
        Label STS1;
        Label Preselec;
        List<Button> clases = new List<Button>();
        Label STS2;
        Label STS3;
        Button Dados;
        string opcion = "";
        int[] STSBase = new int[8];
        string[] STSNom = { "HP","F", "D", "C", "I", "S", "Ca","In"};

        public Form2()
        {
            InitializeComponent();
            Clases();
        }
        private void Clases()
        {
            this.BackColor = Color.FromArgb(222, 184, 135); 
            int margenSuperior = 20;
            int margenIzquierdo = 20;
            int alturaBoton = 40;
            int anchoBoton = 120;
            int espacio = 150;

            Panel fondo = new Panel();
            fondo.Enabled = false;
            fondo.BackColor = Color.Transparent;
            fondo.Size = new Size(900, 550);               
            fondo.Location = new Point(0, 80);              
            fondo.BackgroundImage = Cargar("Proyecto_Dnd.Recursos.Eleccion.png");
            fondo.BackgroundImageLayout = ImageLayout.Stretch;
            this.Controls.Add(fondo);
            this.Controls.SetChildIndex(fondo, this.Controls.Count - 1);


            STS1 = new Label();
            STS1.Text = "Haz clik para elegir el personaje";
            STS1.Font = new Font("Papyrus", 12, FontStyle.Bold);
            STS1.ForeColor = Color.DarkOrange;
            STS1.AutoSize = true;
            STS1.Location = new Point(margenIzquierdo, 10);
            this.Controls.Add(STS1);

            Label STS2 = new Label();
            STS2.Text = "Pasa por encima de las clases para ver sus estadísticas clásicas";
            STS2.Font = new Font("Papyrus", 12, FontStyle.Bold);
            STS2.ForeColor = Color.DarkOrange;
            STS2.AutoSize = true;
            STS2.Location = new Point(margenIzquierdo, 40);
            this.Controls.Add(STS2);

            int baseBotonesY = 80;

            Button Clerigo = new Button();
            Clerigo.Text = "Clérigo";
            Clerigo.Font = new Font("Papyrus", 11, FontStyle.Regular);
            Clerigo.Size = new Size(anchoBoton, alturaBoton);
            Clerigo.Location = new Point(margenIzquierdo, baseBotonesY + 0 * espacio);
            Clerigo.BackColor = Color.Black;
            Clerigo.ForeColor = Color.Orange;
            Clerigo.FlatStyle = FlatStyle.Flat;
            Clerigo.FlatAppearance.BorderSize = 0;
            this.Controls.Add(Clerigo);

            Button Mago = new Button();
            Mago.Text = "Mago";
            Mago.Font = new Font("Papyrus", 11, FontStyle.Regular);
            Mago.Size = new Size(anchoBoton, alturaBoton);
            Mago.Location = new Point(margenIzquierdo, baseBotonesY + 1 * espacio);
            Mago.BackColor = Color.Black;
            Mago.ForeColor = Color.Orange;
            Mago.FlatStyle = FlatStyle.Flat;
            Mago.FlatAppearance.BorderSize = 0;
            this.Controls.Add(Mago);

            Button Guerrero = new Button();
            Guerrero.Text = "Guerrero";
            Guerrero.Font = new Font("Papyrus", 11, FontStyle.Regular);
            Guerrero.Size = new Size(anchoBoton, alturaBoton);
            Guerrero.Location = new Point(margenIzquierdo, baseBotonesY + 2 * espacio);
            Guerrero.BackColor = Color.Black;
            Guerrero.ForeColor = Color.Orange;
            Guerrero.FlatStyle = FlatStyle.Flat;
            Guerrero.FlatAppearance.BorderSize = 0;
            this.Controls.Add(Guerrero);

            Button Paladin = new Button();
            Paladin.Text = "Paladín";
            Paladin.Font = new Font("Papyrus", 11, FontStyle.Regular);
            Paladin.Size = new Size(anchoBoton, alturaBoton);
            Paladin.Location = new Point(margenIzquierdo, baseBotonesY + 3 * espacio);
            Paladin.BackColor = Color.Black;
            Paladin.ForeColor = Color.Orange;
            Paladin.FlatStyle = FlatStyle.Flat;
            Paladin.FlatAppearance.BorderSize = 0;
            this.Controls.Add(Paladin);

            Paladin.MouseClick += desicion;
            Clerigo.MouseClick += desicion;
            Mago.MouseClick += desicion;
            Guerrero.MouseClick += desicion;

            Preselec = new Label();
            Preselec.Font = new Font("Papyrus", 10, FontStyle.Regular);
            Preselec.ForeColor = Color.White;
            Preselec.BackColor = Color.FromArgb(180, 0, 0, 0);
            Preselec.AutoSize = true;
            Preselec.Visible = false;
            Preselec.MaximumSize = new Size(300, 0);
            this.Controls.Add(Preselec);

            Clerigo.MouseEnter += (s, e) => MostrarStats("Clérigo");
            Mago.MouseEnter += (s, e) => MostrarStats("Mago");
            Guerrero.MouseEnter += (s, e) => MostrarStats("Guerrero");
            Paladin.MouseEnter += (s, e) => MostrarStats("Paladín");

            Clerigo.MouseLeave += OcultarStats;
            Mago.MouseLeave += OcultarStats;
            Guerrero.MouseLeave += OcultarStats;
            Paladin.MouseLeave += OcultarStats;

            clases.Add(Clerigo);
            clases.Add(Mago);
            clases.Add(Guerrero);
            clases.Add(Paladin);
           
            this.Controls.Add(Clerigo);
            this.Controls.SetChildIndex(Clerigo, 0);

            this.Controls.Add(Mago);
            this.Controls.SetChildIndex(Mago, 0);

            this.Controls.Add(Guerrero);
            this.Controls.SetChildIndex(Guerrero, 0);

            this.Controls.Add(Paladin);
            this.Controls.SetChildIndex(Paladin, 0);

            this.Controls.Add(Preselec);
            this.Controls.SetChildIndex(Preselec, 0);
        }
        private void desicion(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is Button btn)
            {
                opcion = btn.Text;
                STS1.Text = $"Has elegido al {opcion}";

                foreach (var b in clases)
                    b.Enabled = false;

                switch (opcion)
                {
                    case "Clérigo": STSBase = new int[] {5, 11, 5, 12, 7, 13, 9, -4  }; break;
                    case "Guerrero": STSBase = new int[] {7, 10, 14, 11, 7, 10, 5, 0 }; break;
                    case "Mago": STSBase = new int[] {3, 7, 12, 11, 13, 9, 5, -1  }; break;
                    case "Paladín": STSBase = new int[] { 12, 13, 6, 12, 8, 10, 11, -4}; break;
                }

                if (STS2 == null)
                {
                    STS2 = new Label();
                    STS2.Font = new Font("Papyrus", 11, FontStyle.Regular);
                    STS2.ForeColor = Color.White;
                    STS2.BackColor = Color.FromArgb(180, 0, 0, 0);
                    STS2.AutoSize = true;
                    STS2.Location = new Point(400, 100);
                    this.Controls.Add(STS2);
                    this.Controls.SetChildIndex(STS2, 0);
                }

                string textoBase = $"Stats base de {opcion}:\n\n";
                for (int i = 0; i < STSBase.Length; i++)
                    textoBase += $"{STSNom[i]}: {STSBase[i]}\n";

                STS2.Text = textoBase;
                STS2.Visible = true;

                if (Dados == null)
                {
                    Dados = new Button();
                    Dados.Text = "Lanzar dados";
                    Dados.Font = new Font("Papyrus", 11, FontStyle.Bold);
                    Dados.Size = new Size(180, 66);
                    Dados.Location = new Point(400, 420);
                    Dados.BackColor = Color.DarkSlateGray;
                    Dados.ForeColor = Color.White;
                    Dados.FlatStyle = FlatStyle.Flat;
                    Dados.FlatAppearance.BorderSize = 0;
                    Dados.Click += LanzarDados;
                    this.Controls.Add(Dados);
                    this.Controls.SetChildIndex(Dados, 0);
                }
                else
                {
                    Dados.Visible = true;
                }
            }
        }
        private Image Cargar(string nombreCompleto)
        {
            var ensamblado = Assembly.GetExecutingAssembly();
            using (Stream stream = ensamblado.GetManifestResourceStream(nombreCompleto))
            {
                return Image.FromStream(stream);
            }
        }
        private void MostrarStats(string clase)
        {
            string descripcion = "";
            int  HP = 0,F = 0, D = 0, C = 0, I = 0, S = 0, Ca = 0, In=0;
            switch (clase)
            {
                case "Clérigo":
                    descripcion = "Un Clérigo canaliza su poder divino para curar, proteger y castigar, sigue ante cualquier adversidad.";
                    HP = 8; F = 11; D = 5; C = 12; I = 7; S = 13; Ca = 9; In=-4;
                    break;
                case "Mago":
                    descripcion = "Un Mago domina el conocimiento arcano y lanza hechizos destructivos, ante su fulgor los enemigos tiemblan.";
                    HP = 6; F = 7; D = 12; C = 11; I = 13; S = 9; Ca = 5; In=-1;
                    break;
                case "Guerrero":
                    descripcion = "Un Guerrero combate cuerpo a cuerpo con fuerza bruta y disciplina, extremadamente persistente.";
                    HP = 10; F = 10; D = 14; C = 11; I = 7; S = 10; Ca = 5; In=0;
                    break;
                case "Paladín":
                    descripcion = "Un Paladín lucha con fe y espada, protegiendo a cualquier costo, es lo que se conoce como una fuerza indomable.";
                    HP = 15; F = 13; D = 6; C = 12; I = 8; S = 10; Ca = 11; In=-4 ;
                    break;
            }
            string stats = $"Vida(HP): {HP}\nFuerza: {F}\nDestreza: {D}\nConstitución: {C}\nInteligencia: {I}\nSabiduría: {S}\nCarisma: {Ca}\nIniciativa:{In}";
            Preselec.Text = $"{descripcion}\n\n{stats}";
            Point PosMouse = PointToClient(Cursor.Position);
            PosMouse.Offset(20, 20);
            Preselec.Location = PosMouse;
            Preselec.Visible = true;
        }
        private void OcultarStats(object sender, EventArgs e)
        {
            Preselec.Visible = false;
        }
        private void LanzarDados(object sender, EventArgs e)
        {
            Dados.Enabled = false;
            Random rnd = new Random();
            int[] dados = new int[8];
            int[] statsFinal = new int[8];
            for (int i = 0; i < 8; i++)
            {
                dados[i] = rnd.Next(1, 7);
                statsFinal[i] = STSBase[i] + dados[i];
            }
            string resultado = $"Stats finales de {opcion}:\n\n";
            for (int i = 0; i < 8; i++)
            resultado += $"{STSNom[i]}: {STSBase[i]} + {dados[i]} = {statsFinal[i]}\n";
            STS2.Text = resultado;
            STS2.Refresh();
            Application.DoEvents();
            System.Threading.Thread.Sleep(2000); 
            Form3 siguienteForm = new Form3(opcion, statsFinal);
            siguienteForm.Show();
            this.Hide();
        }

    }
}
