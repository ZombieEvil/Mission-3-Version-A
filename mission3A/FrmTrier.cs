// ========================================
// FrmTrier.cs
// ========================================
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace mission3A
{
    public partial class FrmTrier : Form
    {
        private gsb2023Entities mesDonneesEF;
        private BindingSource bindingSource;

        public FrmTrier()
        {
            InitializeComponent();
            mesDonneesEF = new gsb2023Entities();
            bindingSource = new BindingSource();

            // Configuration initiale du DataGridView
            dgvMedecins.AutoGenerateColumns = false;
            dgvMedecins.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMedecins.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            dgvMedecins.DefaultCellStyle.SelectionForeColor = Color.FromArgb(60, 60, 60);
            dgvMedecins.MultiSelect = false;
            dgvMedecins.EnableHeadersVisualStyles = false;
            dgvMedecins.DataSource = bindingSource;

            // Application du style monochrome
            ApplyMonochromeStyle();
            
            // Configuration des événements du menu
            ConfigurerMenuEvents();

            MettreAJourDonnees(false);
        }

        private void ApplyMonochromeStyle()
        {
            // Les nouvelles couleurs vues dans le screenshot
            Color bleuPrincipal = Color.FromArgb(52, 152, 219);      // Bleu pour l'en-tête et le pied de page
            Color bleuClair = Color.FromArgb(9, 132, 227);           // Bleu pour les boutons
            Color vertBouton = Color.FromArgb(0, 184, 148);          // Vert pour le bouton Ajouter
            Color rougeBouton = Color.FromArgb(214, 48, 49);         // Rouge pour le bouton Supprimer
            Color violetBouton = Color.FromArgb(108, 92, 231);       // Violet pour le bouton Voir les rapports
            
            // En-tête et pied de page
            groupBox1.BackColor = bleuPrincipal;
            panelFooter.BackColor = bleuPrincipal;
            
            // Zone de recherche
            panel1.BackColor = Color.WhiteSmoke;
            panel1.BorderStyle = BorderStyle.None;
            
            // Bouton de recherche
            btnRechercher.BackColor = bleuClair;
            btnRechercher.ForeColor = Color.White;
            btnRechercher.FlatStyle = FlatStyle.Flat;
            btnRechercher.FlatAppearance.BorderSize = 0;
            
            // Boutons d'action
            btnModifier.BackColor = bleuClair;
            btnModifier.ForeColor = Color.White;
            btnModifier.FlatStyle = FlatStyle.Flat;
            btnModifier.FlatAppearance.BorderSize = 0;
            
            btnSupprimer.BackColor = rougeBouton;
            btnSupprimer.ForeColor = Color.White;
            btnSupprimer.FlatStyle = FlatStyle.Flat;
            btnSupprimer.FlatAppearance.BorderSize = 0;
            
            btnAjouter.BackColor = vertBouton;
            btnAjouter.ForeColor = Color.White;
            btnAjouter.FlatStyle = FlatStyle.Flat;
            btnAjouter.FlatAppearance.BorderSize = 0;
            
            btnVoirRapports.BackColor = violetBouton;
            btnVoirRapports.ForeColor = Color.White;
            btnVoirRapports.FlatStyle = FlatStyle.Flat;
            btnVoirRapports.FlatAppearance.BorderSize = 0;
            
            // DataGridView
            dgvMedecins.BorderStyle = BorderStyle.None;
            dgvMedecins.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvMedecins.GridColor = Color.White;
            dgvMedecins.BackgroundColor = Color.White;
            
            // En-têtes du DataGridView
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.BackColor = Color.FromArgb(60, 60, 60);
            headerStyle.ForeColor = Color.White;
            headerStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            headerStyle.SelectionBackColor = Color.FromArgb(60, 60, 60);
            headerStyle.SelectionForeColor = Color.White;
            dgvMedecins.ColumnHeadersDefaultCellStyle = headerStyle;
            dgvMedecins.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            
            // Style des cellules
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            cellStyle.SelectionForeColor = Color.FromArgb(60, 60, 60);
            dgvMedecins.DefaultCellStyle = cellStyle;
        }
        
        private void ConfigureWhiteButton(Button btn, Color textColor)
        {
            // Cette méthode n'est plus utilisée car tous les boutons ont maintenant un fond coloré
        }

        private void MettreAJourDonnees(bool avecSelection)
        {
            try
            {
                // Rafraîchir le contexte
                if (mesDonneesEF != null)
                {
                    mesDonneesEF.Dispose();
                }
                mesDonneesEF = new gsb2023Entities();

                string nomRecherche = txtNomMedecin.Text.ToLower().Trim();
                var resultat = mesDonneesEF.medecins.AsQueryable();

                // Filtrer par nom
                if (!string.IsNullOrEmpty(nomRecherche))
                {
                    resultat = resultat.Where(m => m.nom.ToLower().Contains(nomRecherche));
                }

                // Filtrer par département
                if (!string.IsNullOrEmpty(txtDepartement.Text))
                {
                    if (int.TryParse(txtDepartement.Text, out int departement))
                    {
                        resultat = resultat.Where(m => m.departement == departement);
                    }
                }

                // Trier par nom
                resultat = resultat.OrderBy(m => m.nom);

                // Mise à jour de la source de données
                bindingSource.DataSource = resultat.ToList();

                // Sélection automatique du premier élément si demandé
                if (avecSelection && dgvMedecins.Rows.Count > 0)
                {
                    dgvMedecins.Rows[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la mise à jour des données : " + ex.Message,
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRechercher_Click(object sender, EventArgs e)
        {
            MettreAJourDonnees(true);
        }

        private void btnModifier_Click(object sender, EventArgs e)
        {
            // Modifier
            if (dgvMedecins.SelectedRows.Count > 0)
            {
                var medecinAModifier = dgvMedecins.SelectedRows[0].DataBoundItem as medecin;
                if (medecinAModifier != null)
                {
                    using (var formModification = new EditMed(medecinAModifier))
                    {
                        if (formModification.ShowDialog() == DialogResult.OK)
                        {
                            MettreAJourDonnees(false);
                        }
                    }
                }
            }
        }

        private void btnSupprimer_Click(object sender, EventArgs e)
        {
            // Supprimer
            if (dgvMedecins.SelectedRows.Count > 0)
            {
                try
                {
                    var medecinASupprimer = dgvMedecins.SelectedRows[0].DataBoundItem as medecin;
                    if (medecinASupprimer != null && medecinASupprimer.id != 0)
                    {
                        // Vérifier si le médecin a des rapports associés
                        var rapportsAssocies = mesDonneesEF.rapports.Where(r => r.idMedecin == medecinASupprimer.id).ToList();
                        
                        if (rapportsAssocies.Count > 0)
                        {
                            var message = $"Ce médecin a {rapportsAssocies.Count} rapport(s) associé(s). "
                                + "Voulez-vous supprimer le médecin et tous ses rapports associés ?";
                                
                            if (MessageBox.Show(message, "Confirmation de suppression",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            {
                                // Suppression des rapports liés
                                foreach (var rapport in rapportsAssocies)
                                {
                                    // D'abord, supprimer les offres liées au rapport
                                    var offresLiees = mesDonneesEF.offrirs.Where(o => o.idRapport == rapport.id).ToList();
                                    foreach (var offre in offresLiees)
                                    {
                                        mesDonneesEF.offrirs.Remove(offre);
                                    }
                                    
                                    // Ensuite, supprimer le rapport
                                    mesDonneesEF.rapports.Remove(rapport);
                                }
                                
                                // Supprimer le médecin
                                var medecinASupp = mesDonneesEF.medecins.Find(medecinASupprimer.id);
                                if (medecinASupp != null)
                                {
                                    mesDonneesEF.medecins.Remove(medecinASupp);
                                    mesDonneesEF.SaveChanges();
                                    MettreAJourDonnees(false);
                                    
                                    MessageBox.Show($"Le médecin et ses {rapportsAssocies.Count} rapport(s) ont été supprimés.",
                                        "Suppression réussie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                        else
                        {
                            // Aucun rapport associé, suppression simple
                            if (MessageBox.Show("Voulez-vous vraiment supprimer ce médecin ?",
                                "Confirmation de suppression",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                var medecinASupp = mesDonneesEF.medecins.Find(medecinASupprimer.id);
                                if (medecinASupp != null)
                                {
                                    mesDonneesEF.medecins.Remove(medecinASupp);
                                    mesDonneesEF.SaveChanges();
                                    MettreAJourDonnees(false);
                                    
                                    MessageBox.Show("Le médecin a été supprimé.", 
                                        "Suppression réussie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de la suppression : " + ex.Message,
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAjouter_Click(object sender, EventArgs e)
        {
            // Ajouter - Création d'un nouveau médecin
            try
            {
                // Recherche du prochain ID disponible
                int nouvelId = 1; // ID par défaut si aucun médecin n'existe
                if (mesDonneesEF.medecins.Any())
                {
                    nouvelId = mesDonneesEF.medecins.Max(m => m.id) + 1;
                }

                // Créer un nouveau médecin avec ID = 0 pour forcer le mode ajout
                var nouveauMedecin = new medecin
                {
                    id = 0,  // Forcer le mode ajout avec ID = 0
                    nom = "",
                    prenom = "",
                    adresse = "",
                    tel = "",
                    specialiteComplementaire = "",
                    departement = 0
                };

                // Créer le formulaire en mode ajout
                var formAjout = new EditMed(nouveauMedecin);
                
                // Forcer l'ID calculé dans le médecin à modifier après l'initialisation
                var propriete = formAjout.GetType().GetField("medecinAModifier", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (propriete != null)
                {
                    medecin med = propriete.GetValue(formAjout) as medecin;
                    if (med != null)
                    {
                        med.id = nouvelId;
                    }
                }
                
                // Forcer le mode ajout
                var proprieteMode = formAjout.GetType().GetField("estNouveauMedecin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (proprieteMode != null)
                {
                    proprieteMode.SetValue(formAjout, true);
                }
                
                // Configurer à nouveau le formulaire
                var methodeConfig = formAjout.GetType().GetMethod("ConfigurerFormulaire", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (methodeConfig != null)
                {
                    methodeConfig.Invoke(formAjout, null);
                }
                
                // Appliquer le style
                var methodeStyle = formAjout.GetType().GetMethod("AppliquerStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (methodeStyle != null)
                {
                    methodeStyle.Invoke(formAjout, null);
                }
                
                // Vérifier les états des boutons
                var methodeVerif = formAjout.GetType().GetMethod("VerifierEtatBoutons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (methodeVerif != null)
                {
                    methodeVerif.Invoke(formAjout, null);
                }
                
                // Afficher le formulaire
                if (formAjout.ShowDialog() == DialogResult.OK)
                {
                    // Après ajout, recharger la liste
                    MettreAJourDonnees(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la création du médecin : " + ex.Message,
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtNomMedecin_TextChanged(object sender, EventArgs e)
        {
            MettreAJourDonnees(false);
        }

        private void txtDepartement_TextChanged(object sender, EventArgs e)
        {
            MettreAJourDonnees(false);
        }

        private void btnVoirRapports_Click(object sender, EventArgs e)
        {
            using (var formRapport = new FrmRapport())
            {
                formRapport.ShowDialog();
            }
        }

        private void ConfigurerMenuEvents()
        {
            // Fichier
            quitterToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            quitterToolStripMenuItem.Click += new EventHandler((sender, e) => this.Close());
            
            // Médecins
           // gérerToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.F;
           // gérerToolStripMenuItem.Click += new EventHandler((sender, e) => MettreAJourDonnees(true));
            
            ajouterToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            ajouterToolStripMenuItem.Click += new EventHandler((sender, e) => btnAjouter_Click(sender, e));
            
            modifierToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.M;
            modifierToolStripMenuItem.Click += new EventHandler((sender, e) => btnModifier_Click(sender, e));
            
            supprimerToolStripMenuItem.ShortcutKeys = Keys.Delete;
            supprimerToolStripMenuItem.Click += new EventHandler((sender, e) => btnSupprimer_Click(sender, e));
            
            // Rapports
            voirRapportToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.R;
            voirRapportToolStripMenuItem.Click += new EventHandler((sender, e) => btnVoirRapports_Click(sender, e));
            
            // Aide
            aProposToolStripMenuItem.ShortcutKeys = Keys.F1;
            aProposToolStripMenuItem.Click += new EventHandler((sender, e) => AfficherAPropos());
        }
        
        private void AfficherAPropos()
        {
            MessageBox.Show(
                "GSB - Gestion des médecins\n" +
                "Mission 3a : Rayan BENACHOUR, Manel BENNI, Khelil MIMOUNI\n\n" +
                "Application de gestion des médecins pour GSB\n" +
                "Permettant d'effectuer les opérations suivantes :\n" +
                "- Consultation, ajout, modification et suppression de médecins\n" +
                "- Visualisation des derniers rapports de visite\n" +
                "- Export des rapports en format XML",
                "À propos de l'application",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (mesDonneesEF != null)
            {
                mesDonneesEF.Dispose();
            }
        }

    }
}
