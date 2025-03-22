// ========================================
// EditMed.cs
// ========================================
using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
    
namespace mission3A
{
    public partial class EditMed : Form
    {
        private medecin medecinAModifier;
        private gsb2023Entities mesDonneesEF;
        private bool estNouveauMedecin;

        public EditMed(medecin medecin)
        {
            InitializeComponent();
            mesDonneesEF = new gsb2023Entities();

            // Si l'ID est 0, on considère que c'est un nouveau médecin
            if (medecin.id == 0)
            {
                medecinAModifier = medecin;
                estNouveauMedecin = true;
                
                // Log pour débogage
                Console.WriteLine("Mode ajout détecté - ID: " + medecin.id);
            }
            else
            {
                // Charger depuis la BD pour être sûr d'avoir une version à jour
                var medExist = mesDonneesEF.medecins.Find(medecin.id);
                if (medExist != null)
                {
                    medecinAModifier = medExist;
                    estNouveauMedecin = false;
                    
                    // Log pour débogage
                    Console.WriteLine("Mode modification détecté - ID existant: " + medecin.id);
                }
                else
                {
                    // Si non trouvé, on prend celui fourni, c'est un cas rare
                    medecinAModifier = medecin;
                    
                    // Considérer comme un nouveau médecin si l'ID n'existe pas en base
                    estNouveauMedecin = true;
                    
                    // Log pour débogage
                    Console.WriteLine("Mode ajout détecté - ID non trouvé: " + medecin.id);
                }
            }

            ConfigurerFormulaire();
            ChargerDonneesMedecin();
            AppliquerStyle();
            
            // Un dernier test pour vérifier les états visuels
            VerifierEtatBoutons();
        }

        private void AppliquerStyle()
        {
            // Définition des couleurs
            Color bleuPrincipal = Color.FromArgb(52, 152, 219);
            Color bleuClair = Color.FromArgb(9, 132, 227);
            Color vertBouton = Color.FromArgb(0, 184, 148);
            Color rougeBouton = Color.FromArgb(214, 48, 49);
            
            // Couleur du panel
            panel2.BackColor = bleuPrincipal;
            
            // Couleur du titre
            label7.ForeColor = bleuPrincipal;
            label7.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular);
            
            // Couleurs des boutons
            if (estNouveauMedecin)
            {
                ajouter.BackColor = vertBouton;
                ajouter.ForeColor = Color.White;
                ajouter.FlatStyle = FlatStyle.Flat;
                ajouter.FlatAppearance.BorderSize = 0;
                ajouter.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            }
            else
            {
                modifier.BackColor = bleuClair;
                modifier.ForeColor = Color.White;
                modifier.FlatStyle = FlatStyle.Flat;
                modifier.FlatAppearance.BorderSize = 0;
                modifier.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            }
            
            // Style des labels
            FormatLabel(nom_medecin);
            FormatLabel(prenom_medecin);
            FormatLabel(adresse);
            FormatLabel(num_tel);
            FormatLabel(specialite);
            FormatLabel(departement);
            
            // Fond blanc
            this.BackColor = Color.White;
        }
        
        private void FormatLabel(Label label)
        {
            label.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            label.ForeColor = Color.FromArgb(60, 60, 60);
        }

        private void ConfigurerFormulaire()
        {
            if (estNouveauMedecin)
            {
                this.Text = "Ajouter un nouveau médecin";
                label7.Text = "Ajout d'un nouveau médecin";
                modifier.Visible = false;
                ajouter.Visible = true;
                
                // Log pour débogage
                Console.WriteLine("ConfigurerFormulaire: Mode ajout configuré");
            }
            else
            {
                this.Text = "Modifier un médecin";
                label7.Text = "Modification d'un médecin";
                modifier.Visible = true;
                ajouter.Visible = false;
                
                // Log pour débogage
                Console.WriteLine("ConfigurerFormulaire: Mode modification configuré");
            }
        }

        private void ChargerDonneesMedecin()
        {
            txtnommedecin.Text = medecinAModifier.nom;
            txtprenommedecin.Text = medecinAModifier.prenom;
            txtadresse.Text = medecinAModifier.adresse;
            txtnumtel.Text = medecinAModifier.tel;
            txtdepartement.Text = medecinAModifier.departement.ToString();

            var specialites = mesDonneesEF.medecins
                .Where(m => m.specialiteComplementaire != null && m.specialiteComplementaire != "")
                .Select(m => m.specialiteComplementaire)
                .Distinct()
                .ToList();

            txtspecialite.Items.Clear();
            foreach (var specialite in specialites)
            {
                txtspecialite.Items.Add(specialite);
            }

            if (!string.IsNullOrEmpty(medecinAModifier.specialiteComplementaire))
            {
                int index = txtspecialite.Items.IndexOf(medecinAModifier.specialiteComplementaire);
                if (index >= 0)
                {
                    txtspecialite.SelectedIndex = index;
                }
                else
                {
                    txtspecialite.Items.Add(medecinAModifier.specialiteComplementaire);
                    txtspecialite.SelectedIndex = txtspecialite.Items.Count - 1;
                }
            }
        }

        private bool ValideDonnees()
        {
            if (string.IsNullOrWhiteSpace(txtnommedecin.Text))
            {
                MessageBox.Show("Le nom du médecin est obligatoire", "Erreur de validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtnommedecin.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtprenommedecin.Text))
            {
                MessageBox.Show("Le prénom du médecin est obligatoire", "Erreur de validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtprenommedecin.Focus();
                return false;
            }

            if (!int.TryParse(txtdepartement.Text, out _))
            {
                MessageBox.Show("Le département doit être un nombre valide", "Erreur de validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtdepartement.Focus();
                return false;
            }

            return true;
        }

        private void SauvegarderMedecin()
        {
            if (!ValideDonnees()) return;

            try
            {
                medecinAModifier.nom = txtnommedecin.Text;
                medecinAModifier.prenom = txtprenommedecin.Text;
                medecinAModifier.adresse = txtadresse.Text;
                medecinAModifier.tel = txtnumtel.Text;
                medecinAModifier.specialiteComplementaire = txtspecialite.Text ?? "";
                medecinAModifier.departement = int.Parse(txtdepartement.Text);

                if (estNouveauMedecin)
                {
                    // Vérifier si l'ID est déjà utilisé
                    if (mesDonneesEF.medecins.Any(m => m.id == medecinAModifier.id))
                    {
                        // Si l'ID est déjà utilisé, on cherche le prochain disponible
                        medecinAModifier.id = mesDonneesEF.medecins.Max(m => m.id) + 1;
                    }
                    
                    // Ajout
                    mesDonneesEF.medecins.Add(medecinAModifier);
                }
                else
                {
                    // Modification
                    var medExistant = mesDonneesEF.medecins.Find(medecinAModifier.id);
                    if (medExistant != null)
                    {
                        mesDonneesEF.Entry(medExistant).CurrentValues.SetValues(medecinAModifier);
                    }
                }

                mesDonneesEF.SaveChanges();

                MessageBox.Show(estNouveauMedecin ? "Nouveau médecin ajouté avec succès" : "Modifications enregistrées avec succès",
                    "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'enregistrement : " + ex.Message,
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void modifier_Click(object sender, EventArgs e)
        {
            SauvegarderMedecin();
        }

        private void ajouter_Click(object sender, EventArgs e)
        {
            SauvegarderMedecin();
        }

        private void EditMed_Load(object sender, EventArgs e)
        {
            // Appliquer le style à nouveau pour s'assurer que tout est cohérent
            AppliquerStyle();
            
            // Vérifier que les bons boutons sont visibles
            VerifierEtatBoutons();
            
            // Log pour débogage
            Console.WriteLine("EditMed_Load: Formulaire chargé. Mode ajout: " + estNouveauMedecin);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // Personnalisation du panel si nécessaire
        }

        private void VerifierEtatBoutons()
        {
            // Méthode de vérification pour s'assurer que les bons boutons sont visibles
            if (estNouveauMedecin)
            {
                // Mode ajout
                modifier.Visible = false;
                ajouter.Visible = true;
                
                // Forcer l'application du style pour le bouton Ajouter
                ajouter.BackColor = Color.FromArgb(0, 184, 148);
                ajouter.ForeColor = Color.White;
                ajouter.FlatStyle = FlatStyle.Flat;
                ajouter.FlatAppearance.BorderSize = 0;
                
                // Log pour débogage
                Console.WriteLine("VerifierEtatBoutons: Mode ajout - Bouton ajouter visible");
            }
            else
            {
                // Mode modification
                modifier.Visible = true;
                ajouter.Visible = false;
                
                // Forcer l'application du style pour le bouton Modifier
                modifier.BackColor = Color.FromArgb(9, 132, 227);
                modifier.ForeColor = Color.White;
                modifier.FlatStyle = FlatStyle.Flat;
                modifier.FlatAppearance.BorderSize = 0;
                
                // Log pour débogage
                Console.WriteLine("VerifierEtatBoutons: Mode modification - Bouton modifier visible");
            }
        }
    }
}
