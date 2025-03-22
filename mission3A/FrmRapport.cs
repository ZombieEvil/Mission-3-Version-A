using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Drawing;

namespace mission3A
{
    public partial class FrmRapport : Form
    {
        private gsb2023Entities mesDonneesEF;
        private medecin medecinSelectionne;

        public FrmRapport()
        {
            InitializeComponent();
            mesDonneesEF = new gsb2023Entities();
            ConfigurerFormulaire();
            AppliquerStyle();
        }

        private void ConfigurerFormulaire()
        {
            // Configuration du DataGridView
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.EnableHeadersVisualStyles = false;
            
            // Ajout des colonnes
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "id",
                DataPropertyName = "id",
                HeaderText = "ID",
                Width = 40,
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "nom",
                DataPropertyName = "nom",
                HeaderText = "Nom",
                Width = 100,
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "prenom",
                DataPropertyName = "prenom",
                HeaderText = "Prénom",
                Width = 100,
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "departement",
                DataPropertyName = "departement",
                HeaderText = "Dép.",
                Width = 50,
                ReadOnly = true
            });
        }

        private void AppliquerStyle()
        {
            // Définition des couleurs
            Color bleuPrincipal = Color.FromArgb(52, 152, 219);
            Color bleuClair = Color.FromArgb(9, 132, 227);
            Color vertBouton = Color.FromArgb(0, 184, 148);
            Color rougeBouton = Color.FromArgb(214, 48, 49);
            Color grisFonce = Color.FromArgb(60, 60, 60);
            
            // Styles des contrôles
            dataGridView1.EnableHeadersVisualStyles = false;
            
            // En-têtes du DataGridView
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.BackColor = grisFonce;
            headerStyle.ForeColor = Color.White;
            headerStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            headerStyle.SelectionBackColor = grisFonce;
            headerStyle.SelectionForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle = headerStyle;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            
            // Style des cellules
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.SelectionBackColor = Color.FromArgb(230, 230, 230);
            cellStyle.SelectionForeColor = Color.FromArgb(60, 60, 60);
            dataGridView1.DefaultCellStyle = cellStyle;
            
            // Couleurs des boutons
            btnRechercher.BackColor = bleuClair;
            btnRechercher.ForeColor = Color.White;
            btnRechercher.FlatStyle = FlatStyle.Flat;
            btnRechercher.FlatAppearance.BorderSize = 0;
            
            btnExporter.BackColor = vertBouton;
            btnExporter.ForeColor = Color.White;
            btnExporter.FlatStyle = FlatStyle.Flat;
            btnExporter.FlatAppearance.BorderSize = 0;
            
            btnFermer.BackColor = rougeBouton;
            btnFermer.ForeColor = Color.White;
            btnFermer.FlatStyle = FlatStyle.Flat;
            btnFermer.FlatAppearance.BorderSize = 0;
            
   
        }

        private void RechercherMedecin()
        {
            try
            {
                // Utilisation d'un contexte frais
                using (var ctx = new gsb2023Entities())
                {
                    string nomRecherche = txtNom.Text.ToLower().Trim();
                    string prenomRecherche = txtPrenom.Text.ToLower().Trim();

                    var medecins = ctx.medecins.AsQueryable();

                    if (!string.IsNullOrEmpty(nomRecherche))
                    {
                        medecins = medecins.Where(m => m.nom.ToLower().Contains(nomRecherche));
                    }

                    if (!string.IsNullOrEmpty(prenomRecherche))
                    {
                        medecins = medecins.Where(m => m.prenom.ToLower().Contains(prenomRecherche));
                    }

                    // Si aucun filtre n'est spécifié, limiter les résultats pour éviter de surcharger l'interface
                    if (string.IsNullOrEmpty(nomRecherche) && string.IsNullOrEmpty(prenomRecherche))
                    {
                        medecins = medecins.Take(50); // Limiter à 50 résultats
                    }

                    dataGridView1.DataSource = medecins.OrderBy(m => m.nom).ThenBy(m => m.prenom).ToList();

                    if (dataGridView1.Rows.Count == 0)
                    {
                        txtRapport.Text = "Aucun médecin trouvé avec les critères de recherche spécifiés.";
                    }
                    else if (dataGridView1.Rows.Count == 1)
                    {
                        // Sélectionner automatiquement la ligne si un seul résultat
                        dataGridView1.Rows[0].Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la recherche : " + ex.Message,
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AfficherDernierRapport()
        {
            if (medecinSelectionne == null) return;

            try
            {
                // Utilisation d'un contexte frais pour éviter les problèmes de suivi d'entité
                using (var ctx = new gsb2023Entities())
                {
                    var dernierRapport = ctx.rapports
                        .Where(r => r.idMedecin == medecinSelectionne.id)
                        .OrderByDescending(r => r.date)
                        .FirstOrDefault();

                    if (dernierRapport != null)
                    {
                        txtRapport.Text = $"Date: {dernierRapport.date:dd/MM/yyyy}\r\n" +
                                        $"Motif: {dernierRapport.motif ?? "Non spécifié"}\r\n" +
                                        $"Bilan: {dernierRapport.bilan ?? "Non spécifié"}";
                                        
                        // Chargement explicite des entités associées
                        ctx.Entry(dernierRapport).Collection(r => r.offrirs).Load();
                        
                        // Ajout de la liste des médicaments
                        if (dernierRapport.offrirs.Count > 0)
                        {
                            txtRapport.Text += "\r\n\r\nMédicaments:";
                            foreach (var offre in dernierRapport.offrirs)
                            {
                                // Chargement explicite du médicament associé
                                ctx.Entry(offre).Reference(o => o.medicament).Load();
                                
                                if (offre.medicament != null)
                                {
                                    txtRapport.Text += $"\r\n- {offre.medicament.nomCommercial} (Quantité: {offre.quantite})";
                                }
                            }
                        }
                        else
                        {
                            txtRapport.Text += "\r\n\r\nAucun médicament associé à ce rapport.";
                        }
                    }
                    else
                    {
                        txtRapport.Text = "Aucun rapport trouvé pour ce médecin.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la récupération du rapport : " + ex.Message,
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExporterXML()
        {
            if (medecinSelectionne == null) 
            {
                MessageBox.Show("Veuillez d'abord sélectionner un médecin.", 
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var ctx = new gsb2023Entities())
                {
                    var dernierRapport = ctx.rapports
                        .Where(r => r.idMedecin == medecinSelectionne.id)
                        .OrderByDescending(r => r.date)
                        .FirstOrDefault();

                    if (dernierRapport != null)
                    {
                        // Chargement explicite des entités associées
                        ctx.Entry(dernierRapport).Collection(r => r.offrirs).Load();
                        
                        var medicamentsElement = new XElement("Medicaments");
                        
                        foreach (var offre in dernierRapport.offrirs)
                        {
                            // Chargement explicite du médicament associé
                            ctx.Entry(offre).Reference(o => o.medicament).Load();
                            
                            if (offre.medicament != null)
                            {
                                medicamentsElement.Add(new XElement("Medicament",
                                    new XElement("Nom", offre.medicament.nomCommercial),
                                    new XElement("Quantite", offre.quantite)
                                ));
                            }
                        }
                        
                        var doc = new XDocument(
                            new XElement("Rapport",
                                new XElement("Medecin",
                                    new XElement("Nom", medecinSelectionne.nom),
                                    new XElement("Prenom", medecinSelectionne.prenom)
                                ),
                                new XElement("Date", dernierRapport.date),
                                new XElement("Motif", dernierRapport.motif ?? "Non spécifié"),
                                new XElement("Bilan", dernierRapport.bilan ?? "Non spécifié"),
                                medicamentsElement
                            )
                        );

                        SaveFileDialog saveDialog = new SaveFileDialog
                        {
                            Filter = "Fichiers XML|*.xml",
                            Title = "Enregistrer le rapport",
                            FileName = $"Rapport_{medecinSelectionne.nom}_{DateTime.Now:yyyyMMdd}.xml"
                        };

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            doc.Save(saveDialog.FileName);
                            MessageBox.Show("Rapport exporté avec succès !", "Succès", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Aucun rapport à exporter pour ce médecin.", "Information", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'exportation : " + ex.Message,
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRechercher_Click(object sender, EventArgs e)
        {
            RechercherMedecin();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                medecinSelectionne = dataGridView1.SelectedRows[0].DataBoundItem as medecin;
                AfficherDernierRapport();
            }
        }

        private void btnExporter_Click(object sender, EventArgs e)
        {
            ExporterXML();
        }

        private void btnFermer_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (mesDonneesEF != null)
            {
                mesDonneesEF.Dispose();
            }
        }

        private void FrmRapport_Load(object sender, EventArgs e)
        {
            AppliquerStyle();
        }
    }
} 