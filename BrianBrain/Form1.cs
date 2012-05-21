using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BrianBrain
{
    enum State
    {
        // Null is illegal state
        // represents cells that are out of the border
        Null = -1,
        Passive = 0,
        SemiActive = 1,
        Active = 2
    } ;


    public partial class MainForm : Form
    {
        const int width = 20;
        const int height = 20;
        // by default all cells are Passive
        // array is choosen for fasten access
        State[] board = Enumerable.Repeat(State.Passive, width * height).ToArray();

        public MainForm()
        {
            InitializeComponent();
            while (true)
            {
                var newBoard =
                    from num in Enumerable.Range(0, board.Length).AsParallel()
                    select calculateCellNextState(num);
                // swap iteration
                board = newBoard.ToArray();
            }
        }

        private State T(int x, int y)
        {
            // takes cell from the board with bound checking 
            // (returns State.Null if boudaries were breached)
            return
                Enumerable.Range(0, width).Contains(x) &&
                Enumerable.Range(0, height).Contains(y) ?
                board[x * y]
                : State.Null;
        }

        private State calculateCellNextState(int cell)
        {
            switch (board[cell])
            {
                case State.SemiActive:
                    return State.Passive;
                case State.Active:
                    return State.SemiActive;
                case State.Passive:
                    int col = cell % width;
                    int row = (int)Math.Floor(cell / (double)height);
                    State[] neighborhood = {
                              T(row-1, col-1), T(row-1,col), T(row-1,col+1), 
                              T(row,   col-1), /* [cell]  */ T(row,  col+1), 
                              T(row+1, col-1), T(row+1,col), T(row+1,col+1)
                              };
                    return neighborhood.Count(x => x == State.Active) == 2 ? State.Active : State.Passive;
                default:
                    throw new InvalidOperationException("Matching agains Null cell (cell out of the border)");
            }
        }
    }
}
