using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.IO;


namespace ProjetXmlJSONppe2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        public List<Utilisateur> lesusers = new List<Utilisateur>();
        DataSet monds = new DataSet();
        public MainWindow()
        {
            InitializeComponent();

           
        }


        private void btnlirexmlajoutervoitures_Click(object sender, RoutedEventArgs e)
        {
            monds.ReadXml("occasion.xml");
            dglirexml.ItemsSource = monds.Tables[1].DefaultView;
        }

        private void btnlirexmlenregistrer_Click(object sender, RoutedEventArgs e)
        {
            monds.AcceptChanges();
            monds.WriteXml("occasion.xml", System.Data.XmlWriteMode.WriteSchema);
            MessageBox.Show("Insertion réussite !");

        }

        public bool  VerifierSchema()
        {
            using (StreamReader reader = new StreamReader("SchemaUtilisateur.json"))
            {

                string s = reader.ReadToEnd();
                reader.Close();
                JSchema schema = JSchema.Parse(s);
                string json = @"{'Nom':   '" + tbnomjson.Text + "','Prenom':   '" + tbprenomjson.Text + "','Password':   '" + pbmdpjson.Password + "','Email':   '" + tbmailjson.Text + "'}";
                JObject verif = JObject.Parse(json);
                bool  Res = verif.IsValid(schema);

                return Res ;
            }
        }
        private void OnTabSelected(object sender, RoutedEventArgs e)
        {
            XmlTextReader monlecteurxml = new XmlTextReader("occasion.xml");
            while (monlecteurxml.Read())
                if (monlecteurxml.NodeType == XmlNodeType.Element)
                    if (monlecteurxml.Name == "MARQUE")
                    {
                        string marque = monlecteurxml.ReadElementString();
                        if (lblecteur.Items.Contains(marque) == false)
                            lblecteur.Items.Add(marque);
                    }

            monlecteurxml.Close();
        }
        ProjetXmlJSONppe2.gesperDataSet gesperDataSet;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            gesperDataSet = ((ProjetXmlJSONppe2.gesperDataSet)(this.FindResource("gesperDataSet")));
            // Chargez les données dans la table Salarié. Vous pouvez modifier ce code selon les besoins.
            ProjetXmlJSONppe2.gesperDataSetTableAdapters.SalariéTableAdapter gesperDataSetSalariéTableAdapter = new ProjetXmlJSONppe2.gesperDataSetTableAdapters.SalariéTableAdapter();
            gesperDataSetSalariéTableAdapter.Fill(gesperDataSet.Salarié);
            System.Windows.Data.CollectionViewSource salariéViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("salariéViewSource")));
            salariéViewSource.View.MoveCurrentToFirst();
        }

        private void btnecrirexml_Click(object sender, RoutedEventArgs e)
        {
            gesperDataSet.WriteXml("Saucisse.xml");
        }

        private void OnTabSelected2(object sender, RoutedEventArgs e)
        {
            lbjson.Items.Clear();
            StreamReader r = new StreamReader("Utilisateur.json");
            string json = r.ReadToEnd();
            lesusers = JsonConvert.DeserializeObject<List<Utilisateur>>(json);
            r.Close();

            foreach (Utilisateur user in lesusers)
            {
                lbjson.Items.Add(user.Nom + "   " + user.Prenom + "    " + user.Email);
            }
        }

        private void btneregistrerjson_Click(object sender, RoutedEventArgs e)
        {
 
            if (VerifierSchema() == true )
            { 
                if (tbnomjson.Text != "" && tbprenomjson.Text != "" && tbmailjson.Text != "")
                {
                if (Convert.ToString(pbmdpjson.Password) != "" && Convert.ToString(pbmdpjson.Password) != "12345" && Convert.ToString(pbmdpjson.Password) != "toto"  )
                {
                    Utilisateur leuser;
                    leuser = new Utilisateur(tbnomjson.Text, tbprenomjson.Text, Convert.ToString(pbmdpjson.Password), tbmailjson.Text);
                    lesusers.Add(leuser);
                    lbjson.Items.Clear();                    
                    foreach (Utilisateur user in lesusers)
                    {
                        lbjson.Items.Add(user.Nom + "   " + user.Prenom + "    " + user.Email);
                    }

                        string ajoutfichier;
                        ajoutfichier = JsonConvert.SerializeObject(lesusers);
                        FileStream fs = new FileStream("Utilisateur.json", FileMode.Create);
                        using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8)) { writer.Write(ajoutfichier); }
                        fs.Close();
                        tbnomjson.Clear();
                        tbprenomjson.Clear();
                        pbmdpjson.Clear();
                        tbmailjson.Clear();
                }
                else
                    MessageBox.Show("Veuillez renseigner un mot de passe plus compliqué ", "Erreur mot de passe");
                }
            else
            {
                MessageBox.Show("Veuillez saisir les informations de l'utilisateur ", "Erreur saisie");
            }
            

          
                

           
            }
            else
            {
                MessageBox.Show("Veuillez respectez le schema, le mot de passe doit faire entre 6 et 16 caractères, tout au format chaîne et le mail doit contenir un @", "Erreur schema");
            }
        }

        private void btnsupprimerjson_Click(object sender, RoutedEventArgs e)
        {
            if (lbjson.SelectedIndex != -1)
            {
                if (MessageBox.Show("Êtes vous sûr de vouloir supprimer l'utilisateur sélectionné ?", "Supression", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {


                    lesusers.RemoveAt(lbjson.SelectedIndex);
                    lbjson.Items.Clear();
                    foreach (Utilisateur user in lesusers)
                    {
                        lbjson.Items.Add(user.Nom + "   " + user.Prenom + "    " + user.Email);
                        string ajoutfichier;
                        ajoutfichier = JsonConvert.SerializeObject(lesusers);
                        FileStream fs = new FileStream("Utilisateur.json", FileMode.Create);
                        using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8)) { writer.Write(ajoutfichier); }
                        fs.Close();

                    }
                    tbnomjson.Clear();
                    tbprenomjson.Clear();
                    pbmdpjson.Clear();
                    tbmailjson.Clear();
                }
            }
        }

        private void btnmodifierjson_Click(object sender, RoutedEventArgs e)
        {
            if (lbjson.SelectedIndex != -1)
            {
                if (MessageBox.Show("Êtes vous sûr de vouloir modifier l'utilisateur sélectionné ?", "Modification", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {


                    lesusers.RemoveAt(lbjson.SelectedIndex);
                    Utilisateur modif;
                    modif = new Utilisateur(tbmodifnomjson.Text, tbmodifprenomjson.Text, Convert.ToString(pbmdpmodifjson.Password), tbmodifmailjson.Text);

                    lesusers.Add(modif);
                    lbjson.Items.Clear();
                    foreach (Utilisateur user in lesusers)
                    {
                        lbjson.Items.Add(user.Nom + "   " + user.Prenom + "    " + user.Email);
                        string ajoutfichier;
                        ajoutfichier = JsonConvert.SerializeObject(lesusers);
                        FileStream fs = new FileStream("Utilisateur.json", FileMode.Create);
                        using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8)) { writer.Write(ajoutfichier); }
                        fs.Close();

                    }
                    tbnomjson.Clear();
                    tbprenomjson.Clear();
                    pbmdpjson.Clear();
                    tbmodifmailjson.Clear();
                    tbmodifnomjson.Clear();
                    tbmodifprenomjson.Clear();
                    pbmdpmodifjson.Clear();
                    tbmodifmailjson.Clear();
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un utilisateur sur la liste d'utilisateur", "Erreur selection");
            }

        }

       
        


    }       
}

    
    

