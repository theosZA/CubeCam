﻿using System;
using System.Text;

namespace min2phase
{
    internal class CubieCube
    {
        /**
         * 16 symmetries generated by S_F2, S_U4 and S_LR2
         */
        static CubieCube[] CubeSym = new CubieCube[16];

        /**
         * 18 move cubes
         */
        internal static CubieCube[] moveCube = new CubieCube[18];

        internal static long[] moveCubeSym = new long[18];
        internal static int[] firstMoveSym = new int[48];

        internal static int[] preMove = { -1, Util.Rx1, Util.Rx3, Util.Fx1, Util.Fx3, Util.Lx1, Util.Lx3, Util.Bx1, Util.Bx3 };

        internal static int[] SymInv = new int[16];
        internal static int[,] SymMult = new int[16, 16];
        internal static int[,] SymMove = new int[16, 18];
        internal static int[,] SymMultInv = new int[16, 16];
        internal static int[] Sym8Mult = new int[8 * 8];
        internal static int[] Sym8Move = new int[8 * 18];
        internal static int[] Sym8MultInv = new int[8 * 8];
        internal static int[,] SymMoveUD = new int[16, 10];

        /**
         * ClassIndexToRepresentantArrays
         */
        internal static char[] FlipS2R = new char[336];
        internal static char[] TwistS2R = new char[324];
        internal static char[] EPermS2R = new char[2768];
        internal static int[] UDSliceFlipS2R = Search.EXTRA_PRUN_LEVEL > 0 ? new int[64430] : null;

        /**
         * Notice that Edge Perm Coordnate and Corner Perm Coordnate are the same symmetry structure.
         * So their ClassIndexToRepresentantArray are the same.
         * And when x is RawEdgePermCoordnate, y*16+k is SymEdgePermCoordnate, y*16+(k^e2c[k]) will
         * be the SymCornerPermCoordnate of the State whose RawCornerPermCoordnate is x.
         */
        internal static byte[] e2c = { 0, 0, 0, 0, 1, 3, 1, 3, 1, 3, 1, 3, 0, 0, 0, 0 };

        internal static char[] MtoEPerm = new char[40320];

        internal static int[] FlipSlice2UDSliceFlip = Search.EXTRA_PRUN_LEVEL > 0 ? new int[CoordCube.N_FLIP_SYM * CoordCube.N_SLICE] : null;

        /**
         * Raw-Coordnate to Sym-Coordnate, only for speeding up initializaion.
         */
        internal static char[] FlipR2S;// = new char[2048];
        static char[] TwistR2S;// = new char[2187];
        static char[] EPermR2S;// = new char[40320];
        internal static char[] FlipS2RF = Search.USE_TWIST_FLIP_PRUN ? new char[336 * 8] : null;
        internal static char[] TwistS2RF = Search.EXTRA_PRUN_LEVEL > 0 ? new char[324 * 8] : null;

        /**
         *
         */
        internal static char[] SymStateTwist = new char[324];
        internal static char[] SymStateFlip = new char[336];
        internal static char[] SymStatePerm = new char[2768];
        internal static char[] SymStateUDSliceFlip = Search.EXTRA_PRUN_LEVEL > 0 ? new char[64430] : null;

        static CubieCube urf1 = new CubieCube(2531, 1373, 67026819, 1367);
        static CubieCube urf2 = new CubieCube(2089, 1906, 322752913, 2040);
        internal static byte[,] urfMove = new byte[,] {
        {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17},
        {6, 7, 8, 0, 1, 2, 3, 4, 5, 15, 16, 17, 9, 10, 11, 12, 13, 14},
        {3, 4, 5, 6, 7, 8, 0, 1, 2, 12, 13, 14, 15, 16, 17, 9, 10, 11},
        {2, 1, 0, 5, 4, 3, 8, 7, 6, 11, 10, 9, 14, 13, 12, 17, 16, 15},
        {8, 7, 6, 2, 1, 0, 5, 4, 3, 17, 16, 15, 11, 10, 9, 14, 13, 12},
        {5, 4, 3, 8, 7, 6, 2, 1, 0, 14, 13, 12, 17, 16, 15, 11, 10, 9}
    };

        internal byte[] ca = { 0, 1, 2, 3, 4, 5, 6, 7 };
        internal byte[] ea = { 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22 };
        CubieCube temps = null;

        internal CubieCube()
        {
        }

        internal CubieCube(int cperm, int twist, int eperm, int flip)
        {
            this.setCPerm(cperm);
            this.setTwist(twist);
            Util.setNPerm(ea, eperm, 12, true);
            this.setFlip(flip);
        }

        CubieCube(CubieCube c)
        {
            copy(c);
        }

        public bool equalsCorn(CubieCube c)
        {
            for (int i = 0; i < 8; i++)
            {
                if (ca[i] != c.ca[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool equalsEdge(CubieCube c)
        {
            for (int i = 0; i < 12; i++)
            {
                if (ea[i] != c.ea[i])
                {
                    return false;
                }
            }
            return true;
        }

        void copy(CubieCube c)
        {
            for (int i = 0; i < 8; i++)
            {
                this.ca[i] = c.ca[i];
            }
            for (int i = 0; i < 12; i++)
            {
                this.ea[i] = c.ea[i];
            }
        }

        internal void invCubieCube()
        {
            if (temps == null)
            {
                temps = new CubieCube();
            }
            for (byte edge = 0; edge < 12; edge++)
            {
                temps.ea[ea[edge] >> 1] = (byte)(edge << 1 | ea[edge] & 1);
            }
            for (byte corn = 0; corn < 8; corn++)
            {
                int ori = ca[corn] >> 3;
                ori = 4 >> ori & 3; //0->0, 1->2, 2->1
                temps.ca[ca[corn] & 0x7] = (byte)(corn | ori << 3);
            }
            copy(temps);
        }

        /**
         * prod = a * b, Corner Only.
         */
        internal static void CornMult(CubieCube a, CubieCube b, CubieCube prod)
        {
            for (int corn = 0; corn < 8; corn++)
            {
                int oriA = a.ca[b.ca[corn] & 7] >> 3;
                int oriB = b.ca[corn] >> 3;
                int ori = oriA;
                ori += (oriA < 3) ? oriB : 6 - oriB;
                ori %= 3;
                if ((oriA >= 3) ^ (oriB >= 3))
                {
                    ori += 3;
                }
                prod.ca[corn] = (byte)(a.ca[b.ca[corn] & 7] & 7 | ori << 3);
            }
        }

        /**
         * prod = a * b, Edge Only.
         */
        internal static void EdgeMult(CubieCube a, CubieCube b, CubieCube prod)
        {
            for (int ed = 0; ed < 12; ed++)
            {
                prod.ea[ed] = (byte)(a.ea[b.ea[ed] >> 1] ^ (b.ea[ed] & 1));
            }
        }

        /**
         * b = S_idx^-1 * a * S_idx, Corner Only.
         */
        internal static void CornConjugate(CubieCube a, int idx, CubieCube b)
        {
            CubieCube sinv = CubeSym[SymInv[idx]];
            CubieCube s = CubeSym[idx];
            for (int corn = 0; corn < 8; corn++)
            {
                int oriA = sinv.ca[a.ca[s.ca[corn] & 7] & 7] >> 3;
                int oriB = a.ca[s.ca[corn] & 7] >> 3;
                int ori = (oriA < 3) ? oriB : (3 - oriB) % 3;
                b.ca[corn] = (byte)(sinv.ca[a.ca[s.ca[corn] & 7] & 7] & 7 | ori << 3);
            }
        }

        /**
         * b = S_idx^-1 * a * S_idx, Edge Only.
         */
        internal static void EdgeConjugate(CubieCube a, int idx, CubieCube b)
        {
            CubieCube sinv = CubeSym[SymInv[idx]];
            CubieCube s = CubeSym[idx];
            for (int ed = 0; ed < 12; ed++)
            {
                b.ea[ed] = (byte)(sinv.ea[a.ea[s.ea[ed] >> 1] >> 1] ^ (a.ea[s.ea[ed] >> 1] & 1) ^ (s.ea[ed] & 1));
            }
        }

        /**
         * this = S_urf^-1 * this * S_urf.
         */
        internal void URFConjugate()
        {
            if (temps == null)
            {
                temps = new CubieCube();
            }
            CornMult(urf2, this, temps);
            CornMult(temps, urf1, this);
            EdgeMult(urf2, this, temps);
            EdgeMult(temps, urf1, this);
        }

        // ********************************************* Get and set coordinates *********************************************
        // XSym : Symmetry Coordnate of X. MUST be called after initialization of ClassIndexToRepresentantArrays.

        // ++++++++++++++++++++ Phase 1 Coordnates ++++++++++++++++++++
        // Flip : Orientation of 12 Edges. Raw[0, 2048) Sym[0, 336 * 8)
        // Twist : Orientation of 8 Corners. Raw[0, 2187) Sym[0, 324 * 8)
        // UDSlice : Positions of the 4 UDSlice edges, the order is ignored. [0, 495)

        int getFlip()
        {
            int idx = 0;
            for (int i = 0; i < 11; i++)
            {
                idx = idx << 1 | ea[i] & 1;
            }
            return idx;
        }

        internal void setFlip(int idx)
        {
            int parity = 0;
            for (int i = 10; i >= 0; i--)
            {
                int val = idx & 1;
                ea[i] = (byte)(ea[i] & 0xfe | val);
                parity ^= val;
                idx >>= 1;
            }
            ea[11] = (byte)(ea[11] & 0xfe | parity);
        }

        internal int getFlipSym()
        {
            if (FlipR2S != null)
            {
                return FlipR2S[getFlip()];
            }
            if (temps == null)
            {
                temps = new CubieCube();
            }
            for (int k = 0; k < 16; k += 2)
            {
                EdgeConjugate(this, SymInv[k], temps);
                int idx = Array.BinarySearch(FlipS2R, (char)temps.getFlip());
                if (idx >= 0)
                {
                    return idx << 3 | k >> 1;
                }
            }
            throw new Exception("assert getFlipSym()");
        }

        internal int getTwist()
        {
            int idx = 0;
            for (int i = 0; i < 7; i++)
            {
                idx += (idx << 1) + (ca[i] >> 3);
            }
            return idx;
        }

        internal void setTwist(int idx)
        {
            int twst = 0;
            for (int i = 6; i >= 0; i--)
            {
                int val = idx % 3;
                ca[i] = (byte)(ca[i] & 0x7 | val << 3);
                twst += val;
                idx /= 3;
            }
            ca[7] = (byte)(ca[7] & 0x7 | ((15 - twst) % 3) << 3);
        }

        internal int getTwistSym()
        {
            if (TwistR2S != null)
            {
                return TwistR2S[getTwist()];
            }
            if (temps == null)
            {
                temps = new CubieCube();
            }
            for (int k = 0; k < 16; k += 2)
            {
                CornConjugate(this, SymInv[k], temps);
                int idx = Array.BinarySearch(TwistS2R, (char)temps.getTwist());
                if (idx >= 0)
                {
                    return idx << 3 | k >> 1;
                }
            }
            throw new Exception("assert getTwistSym()");
        }

        internal int getUDSlice()
        {
            return Util.getComb(ea, 8, true);
        }

        internal void setUDSlice(int idx)
        {
            Util.setComb(ea, idx, 8, true);
        }

        internal int getU4Comb()
        {
            return Util.getComb(ea, 0, true);
        }

        internal int getD4Comb()
        {
            return Util.getComb(ea, 4, true);
        }

        // ++++++++++++++++++++ Phase 2 Coordnates ++++++++++++++++++++
        // EPerm : Permutations of 8 UD Edges. Raw[0, 40320) Sym[0, 2187 * 16)
        // Cperm : Permutations of 8 Corners. Raw[0, 40320) Sym[0, 2187 * 16)
        // MPerm : Permutations of 4 UDSlice Edges. [0, 24)

        int getCPerm()
        {
            return Util.get8Perm(ca, false);
        }

        internal void setCPerm(int idx)
        {
            Util.set8Perm(ca, idx, false);
        }

        internal int getCPermSym()
        {
            if (EPermR2S != null)
            {
                int idx = EPermR2S[getCPerm()];
                return idx ^ e2c[idx & 0xf];
            }
            if (temps == null)
            {
                temps = new CubieCube();
            }
            for (int k = 0; k < 16; k++)
            {
                CornConjugate(this, SymInv[k], temps);
                int idx = Array.BinarySearch(EPermS2R, (char)temps.getCPerm());
                if (idx >= 0)
                {
                    return idx << 4 | k;
                }
            }
            throw new Exception("assert getCPermSym()");
        }

        int getEPerm()
        {
            return Util.get8Perm(ea, true);
        }

        internal void setEPerm(int idx)
        {
            Util.set8Perm(ea, idx, true);
        }

        internal int getEPermSym()
        {
            if (EPermR2S != null)
            {
                return EPermR2S[getEPerm()];
            }
            if (temps == null)
            {
                temps = new CubieCube();
            }
            for (int k = 0; k < 16; k++)
            {
                EdgeConjugate(this, SymInv[k], temps);
                int idx = Array.BinarySearch(EPermS2R, (char)temps.getEPerm());
                if (idx >= 0)
                {
                    return idx << 4 | k;
                }
            }
            return 0;
        }

        internal int getMPerm()
        {
            return Util.getComb(ea, 8, true) >> 9;
        }

        internal void setMPerm(int idx)
        {
            Util.setComb(ea, idx << 9, 8, true);
        }

        internal int getCComb()
        {
            return 69 - (Util.getComb(ca, 0, false) & 0x1ff);
        }

        internal void setCComb(int idx)
        {
            Util.setComb(ca, 69 - idx, 0, false);
        }

        /**
         * Check a cubiecube for solvability. Return the error code.
         * 0: Cube is solvable
         * -2: Not all 12 edges exist exactly once
         * -3: Flip error: One edge has to be flipped
         * -4: Not all corners exist exactly once
         * -5: Twist error: One corner has to be twisted
         * -6: Parity error: Two corners or two edges have to be exchanged
         */
        internal int verify()
        {
            int sum = 0;
            int edgeMask = 0;
            for (int e = 0; e < 12; e++)
            {
                edgeMask |= 1 << (ea[e] >> 1);
                sum ^= ea[e] & 1;
            }
            if (edgeMask != 0xfff)
            {
                return -2;// missing edges
            }
            if (sum != 0)
            {
                return -3;
            }
            int cornMask = 0;
            sum = 0;
            for (int c = 0; c < 8; c++)
            {
                cornMask |= 1 << (ca[c] & 7);
                sum += ca[c] >> 3;
            }
            if (cornMask != 0xff)
            {
                return -4;// missing corners
            }
            if (sum % 3 != 0)
            {
                return -5;// twisted corner
            }
            if ((Util.getNParity(Util.getNPerm(ea, 12, true), 12) ^ Util.getNParity(getCPerm(), 8)) != 0)
            {
                return -6;// parity error
            }
            return 0;// cube ok
        }

        internal long selfSymmetry()
        {
            CubieCube c = new CubieCube(this);
            CubieCube d = new CubieCube();
            long sym = 0L;
            for (int i = 0; i < 48; i++)
            {
                CornConjugate(c, SymInv[i % 16], d);
                if (d.equalsCorn(this))
                {
                    EdgeConjugate(c, SymInv[i % 16], d);
                    if (d.equalsEdge(this))
                    {
                        sym |= 1L << i;
                    }
                }
                if (i % 16 == 15)
                {
                    c.URFConjugate();
                }
            }
            c.invCubieCube();
            for (int i = 0; i < 48; i++)
            {
                CornConjugate(c, SymInv[i % 16], d);
                if (d.equalsCorn(this))
                {
                    EdgeConjugate(c, SymInv[i % 16], d);
                    if (d.equalsEdge(this))
                    {
                        sym |= 1L << 48;
                        break;
                    }
                }
                if (i % 16 == 15)
                {
                    c.URFConjugate();
                }
            }
            return sym;
        }

        internal void setUDSliceFlip(int idx)
        {
            setFlip(idx & 0x7ff);
            setUDSlice(idx >> 11);
        }

        int getUDSliceFlip()
        {
            return (getUDSlice() & 0x1ff) << 11 | getFlip();
        }

        internal int getUDSliceFlipSym()
        {
            int flip = getFlipSym();
            int fsym = flip & 0x7;
            flip >>= 3;
            int udslice = getUDSlice() & 0x1ff;
            int udsliceflip = FlipSlice2UDSliceFlip[flip * 495 + CoordCube.UDSliceConj[udslice, fsym]];
            return udsliceflip & unchecked((int)0xfffffff0) | SymMult[udsliceflip & 0xf, fsym << 1];
        }

        // ********************************************* Initialization functions *********************************************

        internal static void initMove()
        {
            moveCube[0] = new CubieCube(15120, 0, 119750400, 0);
            moveCube[3] = new CubieCube(21021, 1494, 323403417, 0);
            moveCube[6] = new CubieCube(8064, 1236, 29441808, 550);
            moveCube[9] = new CubieCube(9, 0, 5880, 0);
            moveCube[12] = new CubieCube(1230, 412, 2949660, 0);
            moveCube[15] = new CubieCube(224, 137, 328552, 137);
            for (int a = 0; a < 18; a += 3)
            {
                for (int p = 0; p < 2; p++)
                {
                    moveCube[a + p + 1] = new CubieCube();
                    EdgeMult(moveCube[a + p], moveCube[a], moveCube[a + p + 1]);
                    CornMult(moveCube[a + p], moveCube[a], moveCube[a + p + 1]);
                }
            }
        }

        public String toString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                sb.Append("|" + (ca[i] & 7) + " " + (ca[i] >> 3));
            }
            sb.Append("\n");
            for (int i = 0; i < 12; i++)
            {
                sb.Append("|" + (ea[i] >> 1) + " " + (ea[i] & 1));
            }
            return sb.ToString();
        }

        internal static void initSym()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            CubieCube t;

            CubieCube f2 = new CubieCube(28783, 0, 259268407, 0);
            CubieCube u4 = new CubieCube(15138, 0, 119765538, 7);
            CubieCube lr2 = new CubieCube(5167, 0, 83473207, 0);
            for (int i = 0; i < 8; i++)
            {
                lr2.ca[i] |= 3 << 3;
            }

            for (int i = 0; i < 16; i++)
            {
                CubeSym[i] = new CubieCube(c);
                CornMult(c, u4, d);
                EdgeMult(c, u4, d);
                t = d; d = c; c = t;
                if (i % 4 == 3)
                {
                    CornMult(c, lr2, d);
                    EdgeMult(c, lr2, d);
                    t = d; d = c; c = t;
                }
                if (i % 8 == 7)
                {
                    CornMult(c, f2, d);
                    EdgeMult(c, f2, d);
                    t = d; d = c; c = t;
                }
            }
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    CornMult(CubeSym[i], CubeSym[j], c);
                    for (int k = 0; k < 16; k++)
                    {
                        if (CubeSym[k].equalsCorn(c))
                        {
                            SymMult[i, j] = k;
                            if (k == 0)
                            {
                                SymInv[i] = j;
                            }
                            break;
                        }
                    }
                }
            }
            for (int j = 0; j < 18; j++)
            {
                for (int s = 0; s < 16; s++)
                {
                    CornConjugate(moveCube[j], SymInv[s], c);
                    for (int m = 0; m < 18; m++)
                    {
                        if (c.equalsCorn(moveCube[m]))
                        {
                            SymMove[s, j] = m;
                            break;
                        }
                    }
                }
            }
            for (int s = 0; s < 16; s++)
            {
                for (int j = 0; j < 10; j++)
                {
                    SymMoveUD[s, j] = Util.std2ud[SymMove[s, Util.ud2std[j]]];
                }
                for (int j = 0; j < 16; j++)
                {
                    SymMultInv[j, s] = SymMult[j, SymInv[s]];
                }
            }
            for (int s = 0; s < 8; s++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Sym8Mult[s << 3 | j] = SymMult[j << 1, s << 1] >> 1;
                    Sym8MultInv[j << 3 | s] = SymMult[j << 1, SymInv[s << 1]] >> 1;
                }
                for (int j = 0; j < 18; j++)
                {
                    Sym8Move[j << 3 | s] = SymMove[s << 1, j];
                }
            }
            for (int i = 0; i < 18; i++)
            {
                moveCubeSym[i] = moveCube[i].selfSymmetry();
            }
            for (int i = 0; i < 18; i++)
            {
                int j = i;
                for (int s = 0; s < 48; s++)
                {
                    if (SymMove[s % 16, j] < i)
                    {
                        firstMoveSym[s] |= 1 << i;
                    }
                    if (s % 16 == 15)
                    {
                        j = urfMove[2, j];
                    }
                }
            }
        }

        internal static void initFlipSym2Raw()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            int count = 0;
            FlipR2S = new char[2048];
            for (int i = 0; i < 2048; i++)
            {
                if (FlipR2S[i] != 0)
                {
                    continue;
                }
                c.setFlip(i);
                for (int s = 0; s < 16; s += 2)
                {
                    EdgeConjugate(c, s, d);
                    int idx = d.getFlip();
                    if (idx == i)
                    {
                        SymStateFlip[count] |= (char)(1 << (s >> 1));
                    }
                    FlipR2S[idx] = (char)(count << 3 | s >> 1);
                    if (Search.USE_TWIST_FLIP_PRUN)
                    {
                        FlipS2RF[count << 3 | s >> 1] = (char)idx;
                    }
                }
                FlipS2R[count++] = (char)i;
            }
            if (count != 336)
                throw new Exception("Unexpected count value");
        }

        internal static void initTwistSym2Raw()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            int count = 0;
            TwistR2S = new char[2187];
            for (int i = 0; i < 2187; i++)
            {
                if (TwistR2S[i] != 0)
                {
                    continue;
                }
                c.setTwist(i);
                for (int s = 0; s < 16; s += 2)
                {
                    CornConjugate(c, s, d);
                    int idx = d.getTwist();
                    if (idx == i)
                    {
                        SymStateTwist[count] |= (char)(1 << (s >> 1));
                    }
                    TwistR2S[idx] = (char)(count << 3 | s >> 1);
                    if (Search.EXTRA_PRUN_LEVEL > 0)
                    {
                        TwistS2RF[count << 3 | s >> 1] = (char)idx;
                    }
                }
                TwistS2R[count++] = (char)i;
            }
            if (count != 324)
                throw new Exception("Unexpected count value");
        }

        internal static byte[] Perm2Comb = new byte[2768];

        internal static void initPermSym2Raw()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            int count = 0;
            EPermR2S = new char[40320];

            for (int i = 0; i < 40320; i++)
            {
                if (EPermR2S[i] != 0)
                {
                    continue;
                }
                c.setEPerm(i);
                for (int s = 0; s < 16; s++)
                {
                    EdgeConjugate(c, s, d);
                    int idx = d.getEPerm();
                    if (idx == i)
                    {
                        SymStatePerm[count] |= (char)(1 << s);
                    }
                    int a = d.getU4Comb();
                    int b = d.getD4Comb() >> 9;
                    int m = 494 - (a & 0x1ff) + (a >> 9) * 70 + b * 1680;
                    MtoEPerm[m] = EPermR2S[idx] = (char)(count << 4 | s);
                    if (s == 0)
                    {
                        Perm2Comb[count] = (byte)(494 - (a & 0x1ff));
                    }
                }
                EPermS2R[count++] = (char)i;
            }
            if (count != 2768)
                throw new Exception("Unexpected count value");
        }

        internal static void initUDSliceFlipSym2Raw()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            int[] occ = new int[2048 * 495 >> 5];
            int count = 0;
            for (int i = 0; i < 2048 * 495; i++)
            {
                if ((occ[i >> 5] & 1 << (i & 0x1f)) != 0)
                {
                    continue;
                }
                c.setUDSliceFlip(i);
                for (int s = 0; s < 16; s++)
                {
                    EdgeConjugate(c, s, d);
                    int idx = d.getUDSliceFlip();
                    if (idx == i)
                    {
                        SymStateUDSliceFlip[count] |= (char)(1 << s);
                    }
                    occ[idx >> 5] |= 1 << (idx & 0x1f);
                    int fidx = Array.BinarySearch(FlipS2R, (char)(idx & 0x7ff));
                    if (fidx >= 0)
                    {
                        FlipSlice2UDSliceFlip[fidx * CoordCube.N_SLICE + (idx >> 11)] = count << 4 | s;
                    }
                }
                UDSliceFlipS2R[count++] = i;
            }
            if (count != 64430)
                throw new Exception("Unexpected count value");
        }
    }
}
