using System;

namespace min2phase
{
    public static class Tools
    {
        private static Random gen = new Random();

        private static void read(byte[] arr, System.IO.BinaryReader input)
        {
            for (int i = 0, len = arr.Length; i < len; i++)
            {
                arr[i] = input.ReadByte();
            }
        }

        private static void read(char[] arr, System.IO.BinaryReader input)
        {
            for (int i = 0, len = arr.Length; i<len; i++) {
                arr[i] = input.ReadChar();
            }
        }

        private static void read(int[] arr, System.IO.BinaryReader input)
        {
            for (int i = 0, len = arr.Length; i < len; i++) {
                arr[i] = input.ReadInt32();
            }
        }

        private static void read(char[,] arr, System.IO.BinaryReader input)
        {
            for (int i = 0, leng = arr.GetLength(0); i < leng; i++) {
                for (int j = 0, len = arr.GetLength(1); j < len; j++)
                {
                    arr[i, j] = input.ReadChar();
                }
            }
        }

        private static void read(int[,] arr, System.IO.BinaryReader input)
        {
            for (int i = 0, leng = arr.GetLength(0); i < leng; i++) {
                for (int j = 0, len = arr.GetLength(1); j < len; j++)
                {
                    arr[i, j] = input.ReadInt32();
                }
            }
        }

        private static void write(byte[] arr, System.IO.BinaryWriter output)
        {
            for (int i = 0, len = arr.Length; i < len; i++)
            {
                output.Write(arr[i]);
            }
        }

        private static void write(char[] arr, System.IO.BinaryWriter output)
        {
            for (int i = 0, len = arr.Length; i < len; i++) {
                output.Write(arr[i]);
            }
        }

        private static void write(int[] arr, System.IO.BinaryWriter output)
        {
            for (int i = 0, len = arr.Length; i < len; i++) {
                output.Write(arr[i]);
            }
        }

        private static void write(char[,] arr, System.IO.BinaryWriter output)
        {
            for (int i = 0, leng = arr.GetLength(0); i < leng; i++) {
                for (int j = 0, len = arr.GetLength(1); j < len; j++)
                {
                    output.Write(arr[i, j]);
                }
            }
        }

        private static void write(int[,] arr, System.IO.BinaryWriter output)
        {
            for (int i = 0, leng = arr.GetLength(0); i < leng; i++) {
                for (int j = 0, len = arr.GetLength(1); j < len; j++)
                {
                    output.Write(arr[i, j]);
                }
            }
        }

        /**
         * initializing from cached tables(move table, pruning table, etc.)
         *
         * @param in
         *     Where to read tables.
         *
         * @see cs.min2phase.Tools#saveTo(java.io.DataOutput)
         */
        public static void initFrom(System.IO.BinaryReader input)
        {
            if (Search.inited) {
                return;
            }
            CubieCube.initMove();
            CubieCube.initSym();
            read(CubieCube.FlipS2R, input);
            read(CubieCube.TwistS2R, input);
            read(CubieCube.EPermS2R, input);
            read(CubieCube.MtoEPerm, input);
            read(CubieCube.Perm2Comb, input);
            read(CoordCube.TwistMove, input);
            read(CoordCube.FlipMove, input);
            read(CoordCube.UDSliceMove, input);
            read(CoordCube.UDSliceConj, input);
            read(CoordCube.CPermMove, input);
            read(CoordCube.EPermMove, input);
            read(CoordCube.MPermMove, input);
            read(CoordCube.MPermConj, input);
            read(CoordCube.CCombMove, input);
            read(CoordCube.CCombConj, input);
            read(CoordCube.MCPermPrun, input);
            read(CoordCube.MEPermPrun, input);
            read(CoordCube.EPermCCombPrun, input);
            if (Search.EXTRA_PRUN_LEVEL > 0) {
                read(CubieCube.UDSliceFlipS2R, input);
                read(CubieCube.TwistS2RF, input);
                read(CoordCubeHuge.TwistMoveF, input);
                read(CoordCubeHuge.TwistConj, input);
                read(CoordCubeHuge.UDSliceFlipMove, input);
                read(CubieCube.FlipSlice2UDSliceFlip, input);
                CoordCubeHuge.initUDSliceFlipTwistPrun();
            } else {
                read(CoordCube.UDSliceTwistPrun, input);
                read(CoordCube.UDSliceFlipPrun, input);
                if (Search.USE_TWIST_FLIP_PRUN)
                {
                    read(CubieCube.FlipS2RF, input);
                    read(CoordCube.TwistFlipPrun, input);
                }
            }
            Search.inited = true;
        }

        /**
         * cache tables (move tables, pruning table, etc.), and read it while initializing.
         *
         * @param out
         *     Where to cache tables.
         *
         * @see cs.min2phase.Tools#initFrom(java.io.DataInput)
         */
        public static void saveTo(System.IO.BinaryWriter output)
        {
            Search.init();
            write(CubieCube.FlipS2R, output);                  //          672
            write(CubieCube.TwistS2R, output);                 // +        648
            write(CubieCube.EPermS2R, output);                 // +      5,536
            write(CubieCube.MtoEPerm, output);                 // +     80,640
            write(CubieCube.Perm2Comb, output);                 // +      2,768
            write(CoordCube.TwistMove, output);                // +     11,664
            write(CoordCube.FlipMove, output);                 // +     12,096
            write(CoordCube.UDSliceMove, output);              // +     17,820
            write(CoordCube.UDSliceConj, output);              // +      7,920
            write(CoordCube.CPermMove, output);                // +     99,648
            write(CoordCube.EPermMove, output);                // +     55,360
            write(CoordCube.MPermMove, output);                // +        480
            write(CoordCube.MPermConj, output);                // +        768
            write(CoordCube.CCombMove, output);                // +      1,400
            write(CoordCube.CCombConj, output);                // +      2,240
            write(CoordCube.MCPermPrun, output);               // +     33,216
            write(CoordCube.MEPermPrun, output);               // +     33,216
            write(CoordCube.EPermCCombPrun, output);           // +     96,880
            if (Search.EXTRA_PRUN_LEVEL > 0) {
                write(CubieCube.UDSliceFlipS2R, output);       // +    257,720
                write(CubieCube.TwistS2RF, output);            // +      5,184
                write(CoordCubeHuge.TwistMoveF, output);
                write(CoordCubeHuge.TwistConj, output);            // +     69,984
                write(CoordCubeHuge.UDSliceFlipMove, output);
                write(CubieCube.FlipSlice2UDSliceFlip, output);// +    665,280
            } else {                                        //
                write(CoordCube.UDSliceTwistPrun, output);     // +     80,192
                write(CoordCube.UDSliceFlipPrun, output);      // +     83,160
                if (Search.USE_TWIST_FLIP_PRUN)
                {           // =    626,324 Bytes
                    write(CubieCube.FlipS2RF, output);         // +      5,376
                    write(CoordCube.TwistFlipPrun, output);    // +    331,776
                }                                           // =    963,476 Bytes
            }
        }

        /**
         * Set Random Source.
         * @param gen new random source.
         */
        public static void setRandomSource(Random gen)
        {
            Tools.gen = gen;
        }

        /**
         * Generates a random cube.<br>
         *
         * The random source can be set by {@link cs.min2phase.Tools#setRandomSource(java.util.Random)}
         *
         * @return A random cube in the string representation. Each cube of the cube space has almost (depends on randomSource) the same probability.
         *
         * @see cs.min2phase.Tools#setRandomSource(java.util.Random)
         * @see cs.min2phase.Search#solution(java.lang.String facelets, int maxDepth, long timeOut, long timeMin, int verbose)
         */
        public static string randomCube()
        {
            return randomState(STATE_RANDOM, STATE_RANDOM, STATE_RANDOM, STATE_RANDOM);
        }

        private static int resolveOri(byte[] arr, int base_)
        {
            int sum = 0, idx = 0, lastUnknown = -1;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == unchecked((byte)-1))
                {
                    arr[i] = (byte)gen.Next(base_);
                    lastUnknown = i;
                }
                sum += arr[i];
            }
            if (sum % base_ != 0 && lastUnknown != -1)
            {
                arr[lastUnknown] = (byte)((30 + arr[lastUnknown] - sum) % base_);
            }
            for (int i = 0; i < arr.Length - 1; i++)
            {
                idx *= base_;
                idx += arr[i];
            }
            return idx;
        }

        private static int countUnknown(byte[] arr)
        {
            if (arr == STATE_SOLVED)
            {
                return 0;
            }
            int cnt = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == unchecked((byte)-1))
                {
                    cnt++;
                }
            }
            return cnt;
        }

        private static int resolvePerm(byte[] arr, int cntU, int parity)
        {
            if (arr == STATE_SOLVED)
            {
                return 0;
            }
            else if (arr == STATE_RANDOM)
            {
                return parity == -1 ? gen.Next(2) : parity;
            }
            byte[] val = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != unchecked((byte)-1))
                {
                    val[arr[i]] = unchecked((byte)-1);
                }
            }
            int idx = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (val[i] != unchecked((byte)-1))
                {
                    int j = gen.Next(idx + 1);
                    byte temp = val[i];
                    val[idx++] = val[j];
                    val[j] = temp;
                }
            }
            int last = -1;
            for (idx = 0; idx < arr.Length && cntU > 0; idx++)
            {
                if (arr[idx] == unchecked((byte)-1))
                {
                    if (cntU == 2)
                    {
                        last = idx;
                    }
                    arr[idx] = val[--cntU];
                }
            }
            int p = Util.getNParity(getNPerm(arr, arr.Length), arr.Length);
            if (p == 1 - parity && last != -1)
            {
                byte temp = arr[idx - 1];
                arr[idx - 1] = arr[last];
                arr[last] = temp;
            }
            return p;
        }

        static int getNPerm(byte[] arr, int n)
        {
            int idx = 0;
            for (int i = 0; i < n; i++)
            {
                idx *= (n - i);
                for (int j = i + 1; j < n; j++)
                {
                    if (arr[j] < arr[i])
                    {
                        idx++;
                    }
                }
            }
            return idx;
        }

        static byte[] STATE_RANDOM = null;
        static byte[] STATE_SOLVED = new byte[0];

        static String randomState(byte[] cp, byte[] co, byte[] ep, byte[] eo)
        {
            int parity;
            int cntUE = ep == STATE_RANDOM ? 12 : countUnknown(ep);
            int cntUC = cp == STATE_RANDOM ? 8 : countUnknown(cp);
            int cpVal, epVal;
            if (cntUE < 2)
            {    //ep != STATE_RANDOM
                if (ep == STATE_SOLVED)
                {
                    epVal = parity = 0;
                }
                else
                {
                    parity = resolvePerm(ep, cntUE, -1);
                    epVal = getNPerm(ep, 12);
                }
                if (cp == STATE_SOLVED)
                {
                    cpVal = 0;
                }
                else if (cp == STATE_RANDOM)
                {
                    do
                    {
                        cpVal = gen.Next(40320);
                    } while (Util.getNParity(cpVal, 8) != parity);
                }
                else
                {
                    resolvePerm(cp, cntUC, parity);
                    cpVal = getNPerm(cp, 8);
                }
            }
            else
            {    //ep != STATE_SOLVED
                if (cp == STATE_SOLVED)
                {
                    cpVal = parity = 0;
                }
                else if (cp == STATE_RANDOM)
                {
                    cpVal = gen.Next(40320);
                    parity = Util.getNParity(cpVal, 8);
                }
                else
                {
                    parity = resolvePerm(cp, cntUC, -1);
                    cpVal = getNPerm(cp, 8);
                }
                if (ep == STATE_RANDOM)
                {
                    do
                    {
                        epVal = gen.Next(479001600);
                    } while (Util.getNParity(epVal, 12) != parity);
                }
                else
                {
                    resolvePerm(ep, cntUE, parity);
                    epVal = getNPerm(ep, 12);
                }
            }
            return Util.toFaceCube(
                       new CubieCube(
                           cpVal,
                           co == STATE_RANDOM ? gen.Next(2187) : (co == STATE_SOLVED ? 0 : resolveOri(co, 3)),
                           epVal,
                           eo == STATE_RANDOM ? gen.Next(2048) : (eo == STATE_SOLVED ? 0 : resolveOri(eo, 2))));
        }


        public static String randomLastLayer()
        {
            return randomState(
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 4, 5, 6, 7 },
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 0, 0, 0, 0 },
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 4, 5, 6, 7, 8, 9, 10, 11 },
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 0, 0, 0, 0, 0, 0, 0, 0 });
        }

        public static String randomLastSlot()
        {
            return randomState(
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 5, 6, 7 },
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 0, 0, 0 },
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 4, 5, 6, 7, unchecked((byte)-1), 9, 10, 11 },
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 0, 0, 0, 0, unchecked((byte)-1), 0, 0, 0 });
        }

        public static String randomZBLastLayer()
        {
            return randomState(
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 4, 5, 6, 7 },
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 0, 0, 0, 0 },
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 4, 5, 6, 7, 8, 9, 10, 11 },
                       STATE_SOLVED);
        }

        public static String randomCornerOfLastLayer()
        {
            return randomState(
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 4, 5, 6, 7 },
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 0, 0, 0, 0 },
                       STATE_SOLVED,
                       STATE_SOLVED);
        }

        public static String randomEdgeOfLastLayer()
        {
            return randomState(
                       STATE_SOLVED,
                       STATE_SOLVED,
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 4, 5, 6, 7, 8, 9, 10, 11 },
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 0, 0, 0, 0, 0, 0, 0, 0 });
        }

        public static String randomCrossSolved()
        {
            return randomState(
                       STATE_RANDOM,
                       STATE_RANDOM,
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 4, 5, 6, 7, unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1) },
                       new byte[] { unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), 0, 0, 0, 0, unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1), unchecked((byte)-1) });
        }

        public static String randomEdgeSolved()
        {
            return randomState(
                       STATE_RANDOM,
                       STATE_RANDOM,
                       STATE_SOLVED,
                       STATE_SOLVED);
        }

        public static String randomCornerSolved()
        {
            return randomState(
                       STATE_SOLVED,
                       STATE_SOLVED,
                       STATE_RANDOM,
                       STATE_RANDOM);
        }

        public static String superFlip()
        {
            return Util.toFaceCube(new CubieCube(0, 0, 0, 2047));
        }


        public static String fromScramble(int[] scramble)
        {
            CubieCube c1 = new CubieCube();
            CubieCube c2 = new CubieCube();
            CubieCube tmp;
            for (int i = 0; i < scramble.Length; i++)
            {
                CubieCube.CornMult(c1, CubieCube.moveCube[scramble[i]], c2);
                CubieCube.EdgeMult(c1, CubieCube.moveCube[scramble[i]], c2);
                tmp = c1; c1 = c2; c2 = tmp;
            }
            return Util.toFaceCube(c1);
        }

        /**
         * Convert a scramble string to its cube definition string.
         *
         * @param s  scramble string. Only outer moves (U, R, F, D, L, B, U2, R2, ...) are recognized.
         * @return cube definition string, which represent the state generated by the scramble<br>
         */
        public static String fromScramble(String s)
        {
            int[] arr = new int[s.Length];
            int j = 0;
            int axis = -1;
            for (int i = 0, length = s.Length; i < length; i++)
            {
                switch (s[i])
                {
                    case 'U': axis = 0; break;
                    case 'R': axis = 3; break;
                    case 'F': axis = 6; break;
                    case 'D': axis = 9; break;
                    case 'L': axis = 12; break;
                    case 'B': axis = 15; break;
                    case ' ':
                        if (axis != -1)
                        {
                            arr[j++] = axis;
                        }
                        axis = -1;
                        break;
                    case '2': axis++; break;
                    case '\'': axis += 2; break;
                    default: continue;
                }

            }
            if (axis != -1) arr[j++] = axis;
            int[] ret = new int[j];
            while (--j >= 0)
            {
                ret[j] = arr[j];
            }
            return fromScramble(ret);
        }

        /**
         * Check whether the cube definition string s represents a solvable cube.
         *
         * @param facelets is the cube definition string , see {@link cs.min2phase.Search#solution(java.lang.String facelets, int maxDepth, long timeOut, long timeMin, int verbose)}
         * @return 0: Cube is solvable<br>
         *         -1: There is not exactly one facelet of each colour<br>
         *         -2: Not all 12 edges exist exactly once<br>
         *         -3: Flip error: One edge has to be flipped<br>
         *         -4: Not all 8 corners exist exactly once<br>
         *         -5: Twist error: One corner has to be twisted<br>
         *         -6: Parity error: Two corners or two edges have to be exchanged
         */
        public static int verify(String facelets)
        {
            return new Search().verify(facelets);
        }
    }
}
