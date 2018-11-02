using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;

namespace ExcelToJson
{
    public class Program
    {
        private const string excelFile = @"D:\XML_MasterTag.20180925.xlsx";
        private const string outputJsonFile = @"D:\outputJson.txt";
        private const string outputJsonDescriptionFile = @"D:\outputDescJson.txt";

        static void Main(string[] args)
        {
            string con = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                + excelFile
                + ";Extended Properties='Excel 12.0;HDR=Yes;';";

            var bettypes = new List<Item>();
            using (OleDbConnection connection = new OleDbConnection(con))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [bettype$]", connection);
                using (OleDbDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        try
                        {
                            if (IsGroupRow(dr))
                            {
                                bettypes.Add(new Item
                                {
                                    BettypeName = dr[0].ToString(),
                                    BettypeId = int.Parse(dr[1].ToString()),
                                    Tag = dr[3].ToString(),
                                    MatchAttribute = dr[5].ToString(),
                                    ExtraAttribute = dr[6].ToString(),
                                    BetChoice = dr[7].ToString(),
                                    Description = dr[8].ToString(),
                                    DBColumns = dr[9].ToString(),
                                });
                            }
                            else
                            {
                                bettypes.Add(new Item
                                {
                                    MatchAttribute = dr[5].ToString(),
                                    ExtraAttribute = dr[6].ToString(),
                                    BetChoice = dr[7].ToString(),
                                    Description = dr[8].ToString(),
                                    DBColumns = dr[9].ToString(),
                                });
                            }
                        }
                        catch (Exception e) { }
                    }
                }
            }

            var currentBettypeId = 0;
            for (var i = 0; i < bettypes.Count; i++)
            {
                if (bettypes[i].BettypeId != 0)
                {
                    currentBettypeId = bettypes[i].BettypeId;
                }
                else
                {
                    bettypes[i].BettypeId = currentBettypeId;
                }
            }

            WriteBetTypeSchemaJson(bettypes);
            WriteBetTypeDescJson(bettypes);
        }

        static void WriteBetTypeSchemaJson(List<Item> bettypes)
        {
            var bettypeGroups = bettypes.GroupBy(b => b.BettypeId).Select(g =>
            {
                var extraBetchoice = new BetChoice
                {
                    MatchAttribute = "h",
                    NotConvert = true,
                    DataAttribute = g.First().HasExtraOverUnder ? "hdp1" : string.Empty
                };

                var group = new JsonItem
                {
                    BettypeId = g.First().BettypeId,
                    Tag = g.First().Tag,
                    Reserve = g.First().HasExtraHandicap ? new ReserveItem() : null,
                    BetChoices = g.Select(b =>
                    {
                        var matchAttribute = b.MatchAttribute.ToLower();
                        if (matchAttribute == "h" && (g.First().HasExtraHandicap || g.First().HasExtraOverUnder))
                        {
                            matchAttribute = "home";
                        }

                        return new BetChoice
                        {
                            MatchAttribute = matchAttribute,
                            DataAttribute = (b.IsResource || b.IsResourceX || b.IsResourceY) ? "resourceID" : b.DataAttribute,
                            IsResource = b.IsResource,
                            IsResourceX = b.IsResourceX,
                            IsResourceY = b.IsResourceY
                        };
                    }).ToList()
                };

                if (g.First().HasExtraHandicap || g.First().HasExtraOverUnder)
                {
                    group.BetChoices.Insert(0, extraBetchoice);
                }

                return group;
            });

            var json = JsonConvert.SerializeObject(bettypeGroups.Where(g => g.BettypeId >= 9001).ToArray(), Formatting.Indented);
            System.IO.File.WriteAllText(outputJsonFile, json);
        }

        static void WriteBetTypeDescJson(List<Item> bettypes)
        {
            var bettypeGroups = bettypes.GroupBy(b => b.BettypeId).Select(g =>
            {


                var group = new JasonDescItem
                {
                    BetTypeId = g.First().BettypeId,
                    BetTypeName = g.First().BettypeName + " bet type",
                    Tag = g.First().Tag,
                    BetChoices = g.Select(b =>
                    {
                        var matchAttribute = b.MatchAttribute.ToLower();
                        if (matchAttribute == "h" && (g.First().HasExtraHandicap || g.First().HasExtraOverUnder))
                        {
                            matchAttribute = "home";
                        }

                        return new BetChoiceDesc
                        {
                            Attribute = matchAttribute,
                            Description = b.Description == string.Empty ? "Price for " + b.BetChoice : b.Description
                        };
                    }).ToList(),
                    ExtraDescription = g.First().HasExtraHandicap
                        ? "For indication of handicap 'h', please show the handicap for the favorite team, use '+' or '-' to indicate the home team is the favorite or underdog i.e.<br> &nbsp;&nbsp; + When h<=0, the home team is the favorite.<br> &nbsp;&nbsp; + When h>0, the away team is the favorite."
                        : string.Empty
                };

                if (g.First().HasExtraHandicap)
                {
                    group.BetChoices.Insert(0, new BetChoiceDesc
                    {
                        Attribute = "h",
                        Description = "Handicap line"
                    });
                }
                else if (g.First().HasExtraOverUnder)
                {
                    group.BetChoices.Insert(0, new BetChoiceDesc
                    {
                        Attribute = "h",
                        Description = "Over/Under line"
                    });
                }


                return group;
            });

            var json = JsonConvert.SerializeObject(bettypeGroups.Where(g => g.BetTypeId >= 9001).ToArray(), Formatting.Indented);
            System.IO.File.WriteAllText(outputJsonDescriptionFile, json);
        }

        static bool IsGroupRow(OleDbDataReader dr)
        {
            try
            {
                return dr[0].ToString() != "";
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
