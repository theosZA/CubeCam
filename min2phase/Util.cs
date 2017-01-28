namespace min2phase
{
    internal static class Util
    {
        /*  //Edges
            const byte UR = 0;
            const byte UF = 1;
            const byte UL = 2;
            const byte UB = 3;
            const byte DR = 4;
            const byte DF = 5;
            const byte DL = 6;
            const byte DB = 7;
            const byte FR = 8;
            const byte FL = 9;
            const byte BL = 10;
            const byte BR = 11;

            //Corners
            const byte URF = 0;
            const byte UFL = 1;
            const byte ULB = 2;
            const byte UBR = 3;
            const byte DFR = 4;
            const byte DLF = 5;
            const byte DBL = 6;
            const byte DRB = 7;
        */
        //Moves
        internal const byte Ux1 = 0;
        internal const byte Ux2 = 1;
        internal const byte Ux3 = 2;
        internal const byte Rx1 = 3;
        internal const byte Rx2 = 4;
        internal const byte Rx3 = 5;
        internal const byte Fx1 = 6;
        internal const byte Fx2 = 7;
        internal const byte Fx3 = 8;
        internal const byte Dx1 = 9;
        internal const byte Dx2 = 10;
        internal const byte Dx3 = 11;
        internal const byte Lx1 = 12;
        internal const byte Lx2 = 13;
        internal const byte Lx3 = 14;
        internal const byte Bx1 = 15;
        internal const byte Bx2 = 16;
        internal const byte Bx3 = 17;

        //Facelets
        internal const byte U1 = 0;
        internal const byte U2 = 1;
        internal const byte U3 = 2;
        internal const byte U4 = 3;
        internal const byte U5 = 4;
        internal const byte U6 = 5;
        internal const byte U7 = 6;
        internal const byte U8 = 7;
        internal const byte U9 = 8;
        internal const byte R1 = 9;
        internal const byte R2 = 10;
        internal const byte R3 = 11;
        internal const byte R4 = 12;
        internal const byte R5 = 13;
        internal const byte R6 = 14;
        internal const byte R7 = 15;
        internal const byte R8 = 16;
        internal const byte R9 = 17;
        internal const byte F1 = 18;
        internal const byte F2 = 19;
        internal const byte F3 = 20;
        internal const byte F4 = 21;
        internal const byte F5 = 22;
        internal const byte F6 = 23;
        internal const byte F7 = 24;
        internal const byte F8 = 25;
        internal const byte F9 = 26;
        internal const byte D1 = 27;
        internal const byte D2 = 28;
        internal const byte D3 = 29;
        internal const byte D4 = 30;
        internal const byte D5 = 31;
        internal const byte D6 = 32;
        internal const byte D7 = 33;
        internal const byte D8 = 34;
        internal const byte D9 = 35;
        internal const byte L1 = 36;
        internal const byte L2 = 37;
        internal const byte L3 = 38;
        internal const byte L4 = 39;
        internal const byte L5 = 40;
        internal const byte L6 = 41;
        internal const byte L7 = 42;
        internal const byte L8 = 43;
        internal const byte L9 = 44;
        internal const byte B1 = 45;
        internal const byte B2 = 46;
        internal const byte B3 = 47;
        internal const byte B4 = 48;
        internal const byte B5 = 49;
        internal const byte B6 = 50;
        internal const byte B7 = 51;
        internal const byte B8 = 52;
        internal const byte B9 = 53;

        //Colors
        internal const byte U = 0;
        internal const byte R = 1;
        internal const byte F = 2;
        internal const byte D = 3;
        internal const byte L = 4;
        internal const byte B = 5;

        static byte[,] cornerFacelet = {
            { U9, R1, F3 }, { U7, F1, L3 }, { U1, L1, B3 }, { U3, B1, R3 },
            { D3, F9, R7 }, { D1, L9, F7 }, { D7, B9, L7 }, { D9, R9, B7 }
        };
        static byte[,] edgeFacelet = {
            { U6, R2 }, { U8, F2 }, { U4, L2 }, { U2, B2 }, { D6, R8 }, { D2, F8 },
            { D4, L8 }, { D8, B8 }, { F6, R4 }, { F4, L6 }, { B6, L4 }, { B4, R6 }
        };

        static int[,] Cnk = new int[13, 13];
        static int[] fact = new int[14];
        internal static int[,] permMult = new int[24, 24];
        internal static string[] move2str = {
            "U ", "U2", "U'", "R ", "R2", "R'", "F ", "F2", "F'",
            "D ", "D2", "D'", "L ", "L2", "L'", "B ", "B2", "B'"
        };
        internal static int[] preMove = { -1, Rx1, Rx3, Fx1, Fx3, Lx1, Lx3, Bx1, Bx3 };
        internal static int[] ud2std = { Ux1, Ux2, Ux3, Rx2, Fx2, Dx1, Dx2, Dx3, Lx2, Bx2 };
        internal static int[] std2ud = new int[18];
        internal static bool[,] ckmv2 = new bool[11, 10];

        internal static void toCubieCube(byte[] f, CubieCube ccRet)
        {
            byte ori;
            for (int i = 0; i < 8; i++)
                ccRet.ca[i] = 0;// invalidate corners
            for (int i = 0; i < 12; i++)
                ccRet.ea[i] = 0;// and edges
            byte col1, col2;
            for (byte i = 0; i < 8; i++)
            {
                // get the colors of the cubie at corner i, starting with U/D
                for (ori = 0; ori < 3; ori++)
                    if (f[cornerFacelet[i, ori]] == U || f[cornerFacelet[i, ori]] == D)
                        break;
                col1 = f[cornerFacelet[i, (ori + 1) % 3]];
                col2 = f[cornerFacelet[i, (ori + 2) % 3]];

                for (byte j = 0; j < 8; j++)
                {
                    if (col1 == cornerFacelet[j, 1] / 9 && col2 == cornerFacelet[j, 2] / 9)
                    {
                        // in cornerposition i we have cornercubie j
                        ccRet.ca[i] = (byte)(ori % 3 << 3 | j);
                        break;
                    }
                }
            }
            for (byte i = 0; i < 12; i++)
            {
                for (byte j = 0; j < 12; j++)
                {
                    if (f[edgeFacelet[i, 0]] == edgeFacelet[j, 0] / 9
                            && f[edgeFacelet[i, 1]] == edgeFacelet[j, 1] / 9)
                    {
                        ccRet.ea[i] = (byte)(j << 1);
                        break;
                    }
                    if (f[edgeFacelet[i, 0]] == edgeFacelet[j, 1] / 9
                            && f[edgeFacelet[i, 1]] == edgeFacelet[j, 0] / 9)
                    {
                        ccRet.ea[i] = (byte)(j << 1 | 1);
                        break;
                    }
                }
            }
        }

        internal static string toFaceCube(CubieCube cc)
        {
            char[] f = new char[54];
            char[] ts = { 'U', 'R', 'F', 'D', 'L', 'B' };
            for (int i = 0; i < 54; i++)
            {
                f[i] = ts[i / 9];
            }
            for (byte c = 0; c < 8; c++)
            {
                int j = cc.ca[c] & 0x7;// cornercubie with index j is at
                                       // cornerposition with index c
                int ori = cc.ca[c] >> 3;// Orientation of this cubie
                for (byte n = 0; n < 3; n++)
                    f[cornerFacelet[c, (n + ori) % 3]] = ts[cornerFacelet[j, n] / 9];
            }
            for (byte e = 0; e < 12; e++)
            {
                int j = cc.ea[e] >> 1;// edgecubie with index j is at edgeposition
                                      // with index e
                int ori = cc.ea[e] & 1;// Orientation of this cubie
                for (byte n = 0; n < 2; n++)
                    f[edgeFacelet[e, (n + ori) % 2]] = ts[edgeFacelet[j, n] / 9];
            }
            return new string(f);
        }

        internal static int getNParity(int idx, int n)
        {
            int p = 0;
            for (int i = n - 2; i >= 0; i--)
            {
                p ^= idx % (n - i);
                idx /= (n - i);
            }
            return p & 1;
        }

        static byte setVal(int val0, int val, bool isEdge)
        {
            return (byte)(isEdge ? (val << 1 | val0 & 1) : (val | val0 & 0xf8));
        }

        static int getVal(int val0, bool isEdge)
        {
            return isEdge ? val0 >> 1 : val0 & 7;
        }

        internal static void set8Perm(byte[] arr, int idx, bool isEdge)
        {
            int val = 0x76543210;
            for (int i = 0; i < 7; i++)
            {
                int p = fact[7 - i];
                int v = idx / p;
                idx -= v * p;
                v <<= 2;
                arr[i] = setVal(arr[i], (val >> v & 0x7), isEdge);
                int m = (1 << v) - 1;
                val = val & m | val >> 4 & ~m;
            }
            arr[7] = setVal(arr[7], val, isEdge);
        }

        internal static int get8Perm(byte[] arr, bool isEdge)
        {
            int idx = 0;
            int val = 0x76543210;
            for (int i = 0; i < 7; i++)
            {
                int v = getVal(arr[i], isEdge) << 2;
                idx = (8 - i) * idx + (val >> v & 0x7);
                val -= 0x11111110 << v;
            }
            return idx;
        }

        internal static void setNPerm(byte[] arr, int idx, int n, bool isEdge)
        {
            arr[n - 1] = setVal(arr[n - 1], 0, isEdge);
            for (int i = n - 2; i >= 0; i--)
            {
                int arri = idx % (n - i);
                arr[i] = setVal(arr[i], arri, isEdge);
                idx /= (n - i);
                for (int j = i + 1; j < n; j++)
                {
                    int arrj = getVal(arr[j], isEdge);
                    if (arrj >= arri)
                    {
                        arr[j] = setVal(arr[j], ++arrj, isEdge);
                    }
                }
            }
        }

        internal static int getNPerm(byte[] arr, int n, bool isEdge)
        {
            int idx = 0;
            for (int i = 0; i < n; i++)
            {
                idx *= (n - i);
                int arri = getVal(arr[i], isEdge);
                for (int j = i + 1; j < n; j++)
                {
                    int arrj = getVal(arr[j], isEdge);
                    if (arrj < arri)
                    {
                        idx++;
                    }
                }
            }
            return idx;
        }

        internal static int getComb(byte[] arr, int mask, bool isEdge)
        {
            int end = arr.Length - 1;
            int idxC = 0, idxP = 0, r = 4, val = 0x0123;
            for (int i = end; i >= 0; i--)
            {
                int perm = getVal(arr[i], isEdge);
                if ((perm & 0xc) == mask)
                {
                    int v = (perm & 3) << 2;
                    idxP = r * idxP + (val >> v & 0xf);
                    val -= 0x0111 >> (12 - v);
                    idxC += Cnk[i, r--];
                }
            }
            return idxP << 9 | Cnk[arr.Length, 4] - 1 - idxC;
        }

        internal static void setComb(byte[] arr, int idx, int mask, bool isEdge)
        {
            int end = arr.Length - 1;
            int r = 4, fill = end, val = 0x0123;
            int idxC = Cnk[arr.Length, 4] - 1 - (idx & 0x1ff);
            int idxP = idx >> 9;
            for (int i = end; i >= 0; i--)
            {
                if (idxC >= Cnk[i, r])
                {
                    idxC -= Cnk[i, r--];
                    int p = fact[r];
                    int v = idxP / p << 2;
                    idxP %= p;
                    arr[i] = setVal(arr[i], val >> v & 3 | mask, isEdge);
                    int m = (1 << v) - 1;
                    val = val & m | val >> 4 & ~m;
                }
                else
                {
                    if ((fill & 0xc) == mask)
                    {
                        fill -= 4;
                    }
                    arr[i] = setVal(arr[i], fill--, isEdge);
                }
            }
        }

        static Util()
        {
            for (int i = 0; i< 10; i++) {
                std2ud[ud2std[i]] = i;
            }
            for (int i = 0; i< 10; i++) {
                int ix = ud2std[i];
                for (int j = 0; j< 10; j++) {
                    int jx = ud2std[j];
                    ckmv2[i, j] = (ix / 3 == jx / 3) || ((ix / 3 % 3 == jx / 3 % 3) && (ix >= jx));
                }
                ckmv2[10, i] = false;
            }
            fact[0] = 1;
            for (int i = 0; i< 13; i++) {
                Cnk[i, 0] = Cnk[i, i] = 1;
                fact[i + 1] = fact[i] * (i + 1);
                for (int j = 1; j<i; j++) {
                    Cnk[i, j] = Cnk[i - 1, j - 1] + Cnk[i - 1, j];
                }
            }
            byte[] arr1 = new byte[4];
            byte[] arr2 = new byte[4];
            byte[] arr3 = new byte[4];
            for (int i = 0; i< 24; i++) {
                setNPerm(arr1, i, 4, false);
                for (int j = 0; j< 24; j++) {
                    setNPerm(arr2, j, 4, false);
                    for (int k = 0; k< 4; k++) {
                        arr3[k] = arr1[arr2[k]];
                    }
                    permMult[i, j] = getNPerm(arr3, 4, false);
                }
            }
        }
    }
}
