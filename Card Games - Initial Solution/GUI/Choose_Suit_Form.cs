using GameObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Games;

namespace GUI
{
    /// <summary>
    /// User choose the suit for card eight
    /// </summary>
    public partial class Choose_Suit_Form : Form
    {
        public Choose_Suit_Form()
        {
            InitializeComponent();
        }

        private void btnPlaycard_Click(object sender, EventArgs e)
        {
            Crazy_Eights_Form.eightSuit = GetChosenSuit();
            this.Close();
        }

        private void rdoChooseSuit(object sender, EventArgs e)
        {
            RadioButton rdoclick = (RadioButton)sender;
            btnPlaycard.Enabled = true;
        }

        /// <summary>
        /// Change suit for card eight
        /// </summary>
        /// <returns>chosen suit for card eight</returns>
        public Suit GetChosenSuit(){
            Suit chosenSuit = new Suit();
            if (rdoClubs.Checked){
                chosenSuit = Suit.Clubs;
            } else if (rdoDiamonds.Checked){
                chosenSuit = Suit.Diamonds;
            } else if (rdoHearts.Checked){
                chosenSuit = Suit.Hearts;
            } else if (rdoSpades.Checked){
                chosenSuit = Suit.Spades;
            }
            return chosenSuit;
        }

    }
}
