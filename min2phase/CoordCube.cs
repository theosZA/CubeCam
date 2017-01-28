using System;

namespace min2phase
{
    internal class CoordCube
    {
        internal const int N_MOVES = 18;
        internal const int N_MOVES2 = 10;

        internal const int N_SLICE = 495;
        internal const int N_TWIST = 2187;
        internal const int N_TWIST_SYM = 324;
        internal const int N_FLIP = 2048;
        internal const int N_FLIP_SYM = 336;
        internal const int N_PERM = 40320;
        internal const int N_PERM_SYM = 2768;
        internal const int N_MPERM = 24;
        internal const int N_COMB = 70;

        //XMove = Move Table
        //XPrun = Pruning Table
        //XConj = Conjugate Table

        //phase1
        internal static char[,] UDSliceMove = new char[N_SLICE, N_MOVES];
        internal static char[,] TwistMove = new char[N_TWIST_SYM, N_MOVES];
        internal static char[,] FlipMove = new char[N_FLIP_SYM, N_MOVES];
        internal static char[,] UDSliceConj = new char[N_SLICE, 8];
        internal static int[] UDSliceTwistPrun = new int[N_SLICE * N_TWIST_SYM / 8 + 1];
        internal static int[] UDSliceFlipPrun = new int[N_SLICE * N_FLIP_SYM / 8];
        internal static int[] TwistFlipPrun = Search.USE_TWIST_FLIP_PRUN ? new int[N_FLIP * N_TWIST_SYM / 8] : null;

        //phase2
        internal static char[,] CPermMove = new char[N_PERM_SYM, N_MOVES];
        internal static char[,] EPermMove = new char[N_PERM_SYM, N_MOVES2];
        internal static char[,] MPermMove = new char[N_MPERM, N_MOVES2];
        internal static char[,] MPermConj = new char[N_MPERM, 16];
        internal static char[,] CCombMove = new char[N_COMB, N_MOVES];
        internal static char[,] CCombConj = new char[N_COMB, 16];
        internal static int[] MCPermPrun = new int[N_MPERM * N_PERM_SYM / 8];
        internal static int[] MEPermPrun = new int[N_MPERM * N_PERM_SYM / 8];
        internal static int[] EPermCCombPrun = new int[N_COMB * N_PERM_SYM / 8];

        internal static void init()
        {
            CubieCube.initPermSym2Raw();

            initCPermMove();
            initEPermMove();
            initMPermMoveConj();
            initCombMoveConj();

            initMEPermPrun();
            initMCPermPrun();
            initPermCombPrun();

            CubieCube.initFlipSym2Raw();
            CubieCube.initTwistSym2Raw();
            initFlipMove();
            initTwistMove();
            initUDSliceMoveConj();

            if (Search.USE_TWIST_FLIP_PRUN)
            {
                initTwistFlipPrun();
            }
            initSliceTwistPrun();
            initSliceFlipPrun();
        }

        static void setPruning(int[] table, int index, int value)
        {
            table[index >> 3] ^= (0xf ^ value) << ((index & 7) << 2);
        }

        internal static int getPruning(int[] table, int index)
        {
            return table[index >> 3] >> ((index & 7) << 2) & 0xf;
        }

        protected static void initUDSliceMoveConj()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_SLICE; i++)
            {
                c.setUDSlice(i);
                for (int j = 0; j < N_MOVES; j += 3)
                {
                    CubieCube.EdgeMult(c, CubieCube.moveCube[j], d);
                    UDSliceMove[i, j] = (char)d.getUDSlice();
                }
                for (int j = 0; j < 16; j += 2)
                {
                    CubieCube.EdgeConjugate(c, CubieCube.SymInv[j], d);
                    UDSliceConj[i, j >> 1] = (char)(d.getUDSlice() & 0x1ff);
                }
            }
            for (int i = 0; i < N_SLICE; i++)
            {
                for (int j = 0; j < N_MOVES; j += 3)
                {
                    int udslice = UDSliceMove[i, j];
                    for (int k = 1; k < 3; k++)
                    {
                        int cx = UDSliceMove[udslice & 0x1ff, j];
                        udslice = Util.permMult[udslice >> 9, cx >> 9] << 9 | cx & 0x1ff;
                        UDSliceMove[i, j + k] = (char)udslice;
                    }
                }
            }
        }

        protected static void initFlipMove()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_FLIP_SYM; i++)
            {
                c.setFlip(CubieCube.FlipS2R[i]);
                for (int j = 0; j < N_MOVES; j++)
                {
                    CubieCube.EdgeMult(c, CubieCube.moveCube[j], d);
                    FlipMove[i, j] = (char)d.getFlipSym();
                }
            }
        }

        static void initTwistMove()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_TWIST_SYM; i++)
            {
                c.setTwist(CubieCube.TwistS2R[i]);
                for (int j = 0; j < N_MOVES; j++)
                {
                    CubieCube.CornMult(c, CubieCube.moveCube[j], d);
                    TwistMove[i, j] = (char)d.getTwistSym();
                }
            }
        }

        protected static void initCPermMove()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_PERM_SYM; i++)
            {
                c.setCPerm(CubieCube.EPermS2R[i]);
                for (int j = 0; j < N_MOVES; j++)
                {
                    CubieCube.CornMult(c, CubieCube.moveCube[j], d);
                    CPermMove[i, j] = (char)d.getCPermSym();
                }
            }
        }

        protected static void initEPermMove()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_PERM_SYM; i++)
            {
                c.setEPerm(CubieCube.EPermS2R[i]);
                for (int j = 0; j < N_MOVES2; j++)
                {
                    CubieCube.EdgeMult(c, CubieCube.moveCube[Util.ud2std[j]], d);
                    EPermMove[i, j] = (char)d.getEPermSym();
                }
            }
        }

        protected static void initMPermMoveConj()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_MPERM; i++)
            {
                c.setMPerm(i);
                for (int j = 0; j < N_MOVES2; j++)
                {
                    CubieCube.EdgeMult(c, CubieCube.moveCube[Util.ud2std[j]], d);
                    MPermMove[i, j] = (char)d.getMPerm();
                }
                for (int j = 0; j < 16; j++)
                {
                    CubieCube.EdgeConjugate(c, CubieCube.SymInv[j], d);
                    MPermConj[i, j] = (char)d.getMPerm();
                }
            }
        }

        protected static void initCombMoveConj()
        {
            CubieCube c = new CubieCube();
            CubieCube d = new CubieCube();
            for (int i = 0; i < N_COMB; i++)
            {
                c.setCComb(i);
                for (int j = 0; j < N_MOVES; j++)
                {
                    CubieCube.CornMult(c, CubieCube.moveCube[j], d);
                    CCombMove[i, j] = (char)d.getCComb();
                }
                for (int j = 0; j < 16; j++)
                {
                    CubieCube.CornConjugate(c, CubieCube.SymInv[j], d);
                    CCombConj[i, j] = (char)d.getCComb();
                }
            }
        }

        static void initTwistFlipPrun()
        {
            int depth = 0;
            int done = 1;
            bool inv;
            int select;
            int check;
            const int N_SIZE = N_FLIP * N_TWIST_SYM;
            for (int i = 0; i < N_SIZE / 8; i++)
            {
                TwistFlipPrun[i] = -1;
            }
            setPruning(TwistFlipPrun, 0, 0);

            while (done < N_SIZE)
            {
                inv = depth > 6;
                select = inv ? 0xf : depth;
                check = inv ? depth : 0xf;
                depth++;
                int val = 0;
                for (int i = 0; i < N_SIZE; i++, val >>= 4)
                {
                    if ((i & 7) == 0)
                    {
                        val = TwistFlipPrun[i >> 3];
                        if (!inv && val == -1)
                        {
                            i += 7;
                            continue;
                        }
                    }
                    if ((val & 0xf) != select)
                    {
                        continue;
                    }
                    int twist = i >> 11;
                    int flip = CubieCube.FlipR2S[i & 0x7ff];
                    int fsym = flip & 7;
                    flip >>= 3;
                    for (int m = 0; m < N_MOVES; m++)
                    {
                        int twistx = TwistMove[twist, m];
                        int tsymx = twistx & 7;
                        twistx >>= 3;
                        int flipx = FlipMove[flip, CubieCube.Sym8Move[m << 3 | fsym]];
                        int fsymx = CubieCube.Sym8MultInv[CubieCube.Sym8Mult[flipx & 7 | fsym << 3] << 3 | tsymx];
                        flipx >>= 3;
                        int idx = twistx << 11 | CubieCube.FlipS2RF[flipx << 3 | fsymx];
                        if (getPruning(TwistFlipPrun, idx) != check)
                        {
                            continue;
                        }
                        done++;
                        if (inv)
                        {
                            setPruning(TwistFlipPrun, i, depth);
                            break;
                        }
                        setPruning(TwistFlipPrun, idx, depth);
                        char sym = CubieCube.SymStateTwist[twistx];
                        if (sym == 1)
                        {
                            continue;
                        }
                        for (int k = 0; k < 8; k++)
                        {
                            if ((sym & 1 << k) == 0)
                            {
                                continue;
                            }
                            int idxx = twistx << 11 | CubieCube.FlipS2RF[flipx << 3 | CubieCube.Sym8MultInv[fsymx << 3 | k]];
                            if (getPruning(TwistFlipPrun, idxx) == 0xf)
                            {
                                setPruning(TwistFlipPrun, idxx, depth);
                                done++;
                            }
                        }
                    }
                }
                // System.out.println(String.format("%2d%10d", depth, done));
            }
        }

        static void initRawSymPrun(int[] PrunTable, int INV_DEPTH,
                                   char[,] RawMove, char[,] RawConj,
                                   char[,] SymMove, char[] SymState,
                                   int PrunFlag)
        {
            int SYM_SHIFT = PrunFlag & 0xf;
            bool SymSwitch = ((PrunFlag >> 4) & 1) == 1;
            bool MoveMapSym = ((PrunFlag >> 5) & 1) == 1;
            bool MoveMapRaw = ((PrunFlag >> 6) & 1) == 1;

            int SYM_MASK = (1 << SYM_SHIFT) - 1;
            int N_RAW = RawMove.GetLength(0);
            int N_SYM = SymMove.GetLength(0);
            int N_SIZE = N_RAW * N_SYM;
            int N_MOVES = MoveMapRaw ? 10 : RawMove.GetLength(1);

            for (int i = 0; i < (N_RAW * N_SYM + 7) / 8; i++)
            {
                PrunTable[i] = -1;
            }
            setPruning(PrunTable, 0, 0);

            int depth = 0;
            int done = 1;

            while (done < N_SIZE)
            {
                bool inv = depth > INV_DEPTH;
                int select = inv ? 0xf : depth;
                int check = inv ? depth : 0xf;
                depth++;
                int val = 0;
                for (int i = 0; i < N_SIZE; i++, val >>= 4)
                {
                    if ((i & 7) == 0)
                    {
                        val = PrunTable[i >> 3];
                        if (!inv && val == -1)
                        {
                            i += 7;
                            continue;
                        }
                    }
                    if ((val & 0xf) != select)
                    {
                        continue;
                    }
                    int raw = i % N_RAW;
                    int sym = i / N_RAW;
                    for (int m = 0; m < N_MOVES; m++)
                    {
                        int symx = SymMove[sym, MoveMapSym ? Util.ud2std[m] : m];
                        int rawx = RawConj[RawMove[raw, MoveMapRaw ? Util.ud2std[m] : m] & 0x1ff, symx & SYM_MASK];
                        symx >>= SYM_SHIFT;
                        int idx = symx * N_RAW + rawx;
                        if (getPruning(PrunTable, idx) != check)
                        {
                            continue;
                        }
                        done++;
                        if (inv)
                        {
                            setPruning(PrunTable, i, depth);
                            break;
                        }
                        setPruning(PrunTable, idx, depth);
                        for (int j = 1, symState = SymState[symx]; (symState >>= 1) != 0; j++)
                        {
                            if ((symState & 1) != 1)
                            {
                                continue;
                            }
                            int idxx = symx * N_RAW + RawConj[rawx, j ^ (SymSwitch ? CubieCube.e2c[j] : 0)];
                            if (getPruning(PrunTable, idxx) == 0xf)
                            {
                                setPruning(PrunTable, idxx, depth);
                                done++;
                            }
                        }
                    }
                }
                // System.out.println(String.format("%2d%10d", depth, done));
            }
        }

        static void initSliceTwistPrun()
        {
            initRawSymPrun(
                UDSliceTwistPrun, 6,
                UDSliceMove, UDSliceConj,
                TwistMove, CubieCube.SymStateTwist, 0x3
            );
        }

        static void initSliceFlipPrun()
        {
            initRawSymPrun(
                UDSliceFlipPrun, 6,
                UDSliceMove, UDSliceConj,
                FlipMove, CubieCube.SymStateFlip, 0x3
            );
        }

        protected static void initMEPermPrun()
        {
            initRawSymPrun(
                MEPermPrun, 7,
                MPermMove, MPermConj,
                EPermMove, CubieCube.SymStatePerm, 0x4
            );
        }

        protected static void initMCPermPrun()
        {
            initRawSymPrun(
                MCPermPrun, 10,
                MPermMove, MPermConj,
                CPermMove, CubieCube.SymStatePerm, 0x34
            );
        }

        protected static void initPermCombPrun()
        {
            initRawSymPrun(
                EPermCCombPrun, 8,
                CCombMove, CCombConj,
                EPermMove, CubieCube.SymStatePerm, 0x44
            );
        }


        internal int twist;
        internal int tsym;
        internal int flip;
        internal int fsym;
        internal int slice;
        internal int prun;

        internal CoordCube() { }

        protected void set(CoordCube node)
        {
            this.twist = node.twist;
            this.tsym = node.tsym;
            this.flip = node.flip;
            this.fsym = node.fsym;
            this.slice = node.slice;
            this.prun = node.prun;
        }

        internal virtual void calcPruning(bool isPhase1)
        {
            prun = Math.Max(
                       Math.Max(
                           getPruning(UDSliceTwistPrun,
                                      twist * N_SLICE + UDSliceConj[slice & 0x1ff, tsym]),
                           getPruning(UDSliceFlipPrun,
                                      flip * N_SLICE + UDSliceConj[slice & 0x1ff, fsym])),
                       Search.USE_TWIST_FLIP_PRUN ? getPruning(TwistFlipPrun,
                               twist << 11 | CubieCube.FlipS2RF[flip << 3 | CubieCube.Sym8MultInv[fsym << 3 | tsym]]) : 0);
        }

        internal virtual void set(CubieCube cc)
        {
            twist = cc.getTwistSym();
            flip = cc.getFlipSym();
            slice = cc.getUDSlice();
            tsym = twist & 7;
            twist = twist >> 3;
            fsym = flip & 7;
            flip = flip >> 3;
        }

        /**
         * @return
         *      0: Success
         *      1: Try Next Power
         *      2: Try Next Axis
         */
        internal virtual int doMovePrun(CoordCube cc, int m, bool isPhase1)
        {
            slice = UDSliceMove[cc.slice & 0x1ff, m] & 0x1ff;

            flip = FlipMove[cc.flip, CubieCube.Sym8Move[m << 3 | cc.fsym]];
            fsym = CubieCube.Sym8Mult[flip & 7 | cc.fsym << 3];
            flip >>= 3;

            twist = TwistMove[cc.twist, CubieCube.Sym8Move[m << 3 | cc.tsym]];
            tsym = CubieCube.Sym8Mult[twist & 7 | cc.tsym << 3];
            twist >>= 3;

            prun = Math.Max(
                       Math.Max(
                           getPruning(UDSliceTwistPrun,
                                      twist * N_SLICE + UDSliceConj[slice, tsym]),
                           getPruning(UDSliceFlipPrun,
                                      flip * N_SLICE + UDSliceConj[slice, fsym])),
                       Search.USE_TWIST_FLIP_PRUN ? getPruning(TwistFlipPrun,
                               twist << 11 | CubieCube.FlipS2RF[flip << 3 | CubieCube.Sym8MultInv[fsym << 3 | tsym]]) : 0);
            return prun;
        }
    }
}
