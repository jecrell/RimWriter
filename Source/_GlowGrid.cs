using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Verse;
using RimWorld;
using UnityEngine;

namespace Cthulhu.Detour
{
    internal static class _GlowFlooder
    {
        //    MethodInfo method = typeof(InteractionWorker_RomanceAttempt).GetMethod("SendNewLoversLetter", BindingFlags.Instance | BindingFlags.NonPublic);
        //    bool flag3 = method != null;
        //	if (flag3)
        //	{
        //		method.Invoke(_this, new object[]
        //		{
        //			initiator,
        //			recipient,
        //			list,
        //			list2
        //});
        //	}

        //typeof(Pawn_StoryTracker).GetField("headGraphicPath", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(pawn.story, headGraphicPath);

            
        internal static FieldInfo _calcGrid;
        internal static FieldInfo _glowGrid;
        internal static FieldInfo _glower;
        internal static FieldInfo _unseenVal;
        internal static FieldInfo _openVal;
        internal static FieldInfo _finalizedVal;
        internal static FieldInfo _openSet;
        internal static FieldInfo _gridSizeXMinus1;
        internal static FieldInfo _gridSizeZLog2;
        internal static FieldInfo _map;
        internal static FieldInfo _blockers;

        internal struct GlowFloodCell
            {
                public int intDist;

                public uint status;
            }

        
        internal static GlowFloodCell[] GetCalcGrid(this GlowFlooder _this)
        {
            if (_GlowFlooder._calcGrid == null)
            {
                _GlowFlooder._calcGrid = typeof(GlowFlooder).GetField("calcGrid", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_GlowFlooder._calcGrid == null)
                {
                    Log.ErrorOnce("Unable to reflect GlowFlooder.calcGrid!", 305432421);
                }
            }
            return (GlowFloodCell[])_GlowFlooder._calcGrid.GetValue(_this);
        }
        internal static Color32[] GetGlowGrid(this GlowFlooder _this)
        {
            if (_GlowFlooder._glowGrid == null)
            {
                _GlowFlooder._glowGrid = typeof(GlowFlooder).GetField("glowGrid", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_GlowFlooder._glowGrid == null)
                {
                    Log.ErrorOnce("Unable to reflect GlowFlooder.glowGrid!", 305432421);
                }
            }
            return (Color32[])_GlowFlooder._glowGrid.GetValue(_this);
        }
        internal static Map GetMap(this GlowFlooder _this)
        {
            if (_GlowFlooder._map == null)
            {
                _GlowFlooder._map = typeof(GlowFlooder).GetField("map", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_GlowFlooder._map == null)
                {
                    Log.ErrorOnce("Unable to reflect GlowFlooder.map!", 305432421);
                }
            }
            return (Map)_GlowFlooder._map.GetValue(_this);
        }
        internal static CompGlower GetGlower(this GlowFlooder _this)
        {
            if (_GlowFlooder._glower == null)
            {
                _GlowFlooder._glower = typeof(GlowFlooder).GetField("glower", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_GlowFlooder._glower == null)
                {
                    Log.ErrorOnce("Unable to reflect GlowFlooder.glower!", 305432421);
                }
            }
            return (CompGlower)_GlowFlooder._glower.GetValue(_this);
        }
        internal static uint GetUnseenVal(this GlowFlooder _this)
        {
            if (_GlowFlooder._unseenVal == null)
            {
                _GlowFlooder._unseenVal = typeof(GlowFlooder).GetField("unseenVal", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_GlowFlooder._unseenVal == null)
                {
                    Log.ErrorOnce("Unable to reflect GlowFlooder.unseenVal!", 305432421);
                }
            }
            return (uint)_GlowFlooder._unseenVal.GetValue(_this);
        }
        internal static uint GetOpenVal(this GlowFlooder _this)
        {
            if (_GlowFlooder._openVal == null)
            {
                _GlowFlooder._openVal = typeof(GlowFlooder).GetField("openVal", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_GlowFlooder._openVal == null)
                {
                    Log.ErrorOnce("Unable to reflect GlowFlooder.openVal!", 305432421);
                }
            }
            return (uint)_GlowFlooder._openVal.GetValue(_this);
        }
        internal static uint GetFinalizedVal(this GlowFlooder _this)
        {
            if (_GlowFlooder._finalizedVal == null)
            {
                _GlowFlooder._finalizedVal = typeof(GlowFlooder).GetField("finalizedVal", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_GlowFlooder._finalizedVal == null)
                {
                    Log.ErrorOnce("Unable to reflect GlowFlooder.finalizedVal!", 305432421);
                }
            }
            return (uint)_GlowFlooder._finalizedVal.GetValue(_this);
        }
        internal static FastPriorityQueue<int> GetOpenSet(this GlowFlooder _this)
        {
            if (_GlowFlooder._openSet == null)
            {
                _GlowFlooder._openSet = typeof(GlowFlooder).GetField("openSet", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_GlowFlooder._openSet == null)
                {
                    Log.ErrorOnce("Unable to reflect GlowFlooder.openSet!", 305432421);
                }
            }
            return (FastPriorityQueue<int>)_GlowFlooder._openSet.GetValue(_this);
        }
        internal static ushort GetGridSizeZLog2(this GlowFlooder _this)
        {
            if (_GlowFlooder._gridSizeZLog2 == null)
            {
                _GlowFlooder._gridSizeZLog2 = typeof(GlowFlooder).GetField("gridSizeZLog2", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_GlowFlooder._gridSizeZLog2 == null)
                {
                    Log.ErrorOnce("Unable to reflect GlowFlooder.gridSizeZLog2!", 305432421);
                }
            }
            return (ushort)_GlowFlooder._gridSizeZLog2.GetValue(_this);
        }
        internal static ushort GetGridSizeXMinus1(this GlowFlooder _this)
        {
            if (_GlowFlooder._gridSizeXMinus1 == null)
            {
                _GlowFlooder._gridSizeXMinus1 = typeof(GlowFlooder).GetField("gridSizeXMinus1", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_GlowFlooder._gridSizeXMinus1 == null)
                {
                    Log.ErrorOnce("Unable to reflect GlowFlooder.gridSizeXMinus1!", 305432421);
                }
            }
            return (ushort)_GlowFlooder._gridSizeXMinus1.GetValue(_this);
        }

        internal static Thing[] GetBlockers(this GlowFlooder _this)
        {
            if (_GlowFlooder._blockers == null)
            {
                _GlowFlooder._blockers = typeof(GlowFlooder).GetField("blockers", BindingFlags.Instance | BindingFlags.NonPublic);
                if (_GlowFlooder._blockers == null)
                {
                    Log.ErrorOnce("Unable to reflect GlowFlooder.blockers!", 305432421);
                }
            }
            return (Thing[])_GlowFlooder._blockers.GetValue(_this);
        }

        // Verse.GlowFlooder
        internal static readonly sbyte[,] Directions = new sbyte[,]
        {
    {
        0,
        -1
    },
    {
        1,
        0
    },
    {
        0,
        1
    },
    {
        -1,
        0
    },
    {
        1,
        -1
    },
    {
        1,
        1
    },
    {
        -1,
        1
    },
    {
        -1,
        -1
    }
        };


        [Detour(typeof(GlowFlooder), bindingFlags = (BindingFlags.Instance | BindingFlags.Public))]
        internal static void AddFloodGlowFor(this GlowFlooder g, CompGlower theGlower)
        {
            if (g.GetCalcGrid() == null)
            {
                MethodInfo method = typeof(GlowFlooder).GetMethod("InitializeWorkingData", BindingFlags.Default | BindingFlags.NonPublic);
                method.Invoke(g, new object[] { });
            }
            typeof(GlowFlooder).GetField("glowGrid", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(g, g.GetMap().glowGrid.glowGrid);
            typeof(GlowFlooder).GetField("glower", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(g, theGlower);
            Thing[] innerArray = g.GetMap().edificeGrid.InnerArray;
            typeof(GlowFlooder).GetField("unseenVal", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(g,
                g.GetUnseenVal() + 3u
            );
            typeof(GlowFlooder).GetField("openVal", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(g,
                g.GetOpenVal() + 3u
            );
            typeof(GlowFlooder).GetField("finalizedVal", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(g,
                g.GetFinalizedVal() + 3u
            );

            IntVec3 position = g.GetGlower().parent.Position;
            
            typeof(GlowFlooder).GetField("attenLinearSlope", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(g, -1f / g.GetGlower().Props.glowRadius); 
            int num = Mathf.RoundToInt(g.GetGlower().Props.glowRadius * 100f);
            IntVec3 intVec = default(IntVec3);
            IntVec3 c = default(IntVec3);
            int num2 = 0;
            CellIndices cellIndices = g.GetMap().cellIndices;
            g.GetOpenSet().Clear();
            int num3 = (position.z << (int)g.GetGridSizeZLog2()) + position.x;
            g.GetCalcGrid()[num3].intDist = 100;
            g.GetOpenSet().Push(num3);
            while (g.GetOpenSet().Count != 0)
            {
                int num4 = g.GetOpenSet().Pop();
                intVec.x = (int)((ushort)(num4 & (int)g.GetGridSizeXMinus1()));
                intVec.z = (int)((ushort)(num4 >> (int)g.GetGridSizeZLog2()));
                g.GetCalcGrid()[num4].status = g.GetFinalizedVal();
                MethodInfo method0 = typeof(GlowFlooder).GetMethod("SetGlowGridFromDist", BindingFlags.Default | BindingFlags.NonPublic);
                method0.Invoke(g, new object[] { intVec });
                if (UnityData.isDebugBuild && DebugViewSettings.drawGlow)
                {
                    g.GetMap().debugDrawer.FlashCell(intVec, (float)g.GetCalcGrid()[num4].intDist / 10f, g.GetCalcGrid()[num4].intDist.ToString("F2"));
                    num2++;
                }
                for (int i = 0; i < 8; i++)
                {
                    c.x = (int)((ushort)(intVec.x + (int)_GlowFlooder.Directions[i, 0]));
                    c.z = (int)((ushort)(intVec.z + (int)_GlowFlooder.Directions[i, 1]));
                    int num5 = (c.z << (int)g.GetGridSizeZLog2()) + c.x;
                    if (c.InBounds(g.GetMap()))
                    {
                        if (g.GetCalcGrid()[num5].status != g.GetFinalizedVal())
                        {
                            g.GetBlockers()[i] = innerArray[cellIndices.CellToIndex(c)];
                            if (g.GetBlockers()[i] != null)
                            {
                                if (g.GetBlockers()[i].def.blockLight)
                                {
                                    goto IL_47C;
                                }
                                g.GetBlockers()[i] = null;
                            }
                            int num6;
                            if (i < 4)
                            {
                                num6 = 100;
                            }
                            else
                            {
                                num6 = 141;
                            }
                            int num7 = g.GetCalcGrid()[num4].intDist + num6;
                            if (num7 <= num)
                            {
                                if (g.GetCalcGrid()[num5].status != g.GetFinalizedVal())
                                {
                                    if (i >= 4)
                                    {
                                        bool flag = false;
                                        switch (i)
                                        {
                                            case 4:
                                                if (g.GetBlockers()[0] != null && g.GetBlockers()[1] != null)
                                                {
                                                    flag = true;
                                                }
                                                break;
                                            case 5:
                                                if (g.GetBlockers()[1] != null && g.GetBlockers()[2] != null)
                                                {
                                                    flag = true;
                                                }
                                                break;
                                            case 6:
                                                if (g.GetBlockers()[2] != null && g.GetBlockers()[3] != null)
                                                {
                                                    flag = true;
                                                }
                                                break;
                                            case 7:
                                                if (g.GetBlockers()[0] != null && g.GetBlockers()[3] != null)
                                                {
                                                    flag = true;
                                                }
                                                break;
                                        }
                                        if (flag)
                                        {
                                            goto IL_47C;
                                        }
                                    }
                                    if (g.GetCalcGrid()[num5].status <= g.GetUnseenVal())
                                    {
                                        g.GetCalcGrid()[num5].intDist = 999999;
                                        g.GetCalcGrid()[num5].status = g.GetOpenVal();
                                    }
                                    if (num7 < g.GetCalcGrid()[num5].intDist)
                                    {
                                        g.GetCalcGrid()[num5].intDist = num7;
                                        g.GetCalcGrid()[num5].status = g.GetOpenVal();
                                        g.GetOpenSet().Push(num5);
                                    }
                                }
                            }
                        }
                    }
                    IL_47C:;
                }
            }

        }
    }
}
