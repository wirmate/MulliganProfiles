using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using SmartBot.Plugins;
using SmartBot.Plugins.API;
using SmartBotUI;
using SmartBotUI.Settings;
using CClass = SmartBot.Plugins.API.Card.CClass;

namespace SmartBotUI.Mulligan
{
    [Serializable]
    public class MulliganOpponentDeckTemplate : MulliganProfile
    {
        public MulliganOpponentDeckTemplate()
        {
            // Don't delete this
            SharedDataMgr.OnQueryResultsReceived += OnSharedDataReceived;
        }

        public override List<Card> HandleMulligan(List<Card> Choices, CClass opponentClass, CClass ownClass)
        {
            // Don't delete this
            _deckData = null;
            _playerHistories = null;
            _messageReceived = new AutoResetEvent(false);

            // For testing (replace this and everything below with your stuff)
            var deckData = GetEnemyDeckDataForClass(opponentClass);

            return Choices;
        }

        #region OpponentDeck Stuff

        #region Data

        private DeckData _deckData;
        private List<PlayerHistory> _playerHistories;
        private AutoResetEvent _messageReceived;

        #endregion Data

        /// <summary>
        /// Gets the opponents DeckArchtype based on a calculated guess.
        /// </summary>
        /// <param name="opponentClass"></param>
        /// <returns></returns>
        private DeckData GetEnemyDeckDataForClass(CClass opponentClass)
        {
            if (_deckData == null)
            {
                var playerHistory = GetOpponentHistoryWithClass(opponentClass).FirstOrDefault();

                if (playerHistory != null)
                {
                    _deckData = new DeckData
                    {
                        Cards = playerHistory.Cards
                    };

                    switch (opponentClass)
                    {
                        case CClass.WARRIOR:
                            {
                                // Temporary for testing
                                if (_deckData.Cards.Any(c =>
                                    c == "BRM_019" || // Patron
                                    c == "EX1_392" || // Battle rage
                                    c == "EX1_084")) // Warsong Commander
                                {
                                    _deckData.SubType = SubType.PatronWarrior;
                                    _deckData.ArchType = DeckArchtype.Combo;
                                }
                                else if (_deckData.Cards.Any(c =>
                                    c == "GVG_082" || // Clockwork Gnome
                                    c == "GVG_013" || // Cogmaster
                                    c == "GVG_085" || // Annoy-o-Tron
                                    c == "GVG_006")) // Mechwarper
                                {
                                    _deckData.SubType = SubType.MechWarrior;
                                    _deckData.ArchType = DeckArchtype.Aggro;
                                }
                                else if (_deckData.Cards.Any(c =>
                                  c == "AT_071" || // Alexstrasza's Champion
                                  c == "AT_017" || // Twilight Guardian
                                  c == "BRM_033" || // Blackwing Technician
                                  c == "BRM_034")) // Blackwing Corruptor
                                {
                                    _deckData.SubType = SubType.DragonWarrior;
                                    _deckData.ArchType = DeckArchtype.Midrange;
                                }
                                else
                                {
                                    // Else it's most likely Control Warrior
                                    _deckData.SubType = SubType.ControlWarrior;
                                    _deckData.ArchType = DeckArchtype.Control;
                                }
                                break;
                            }
                        case CClass.ROGUE:
                            {
                                if (_deckData.Cards.Any(c =>
                                    c == "GVG_022")) // Tinker's Sharpsword Oil
                                {
                                    _deckData.SubType = SubType.OilRogue;
                                    _deckData.ArchType = DeckArchtype.Combo; // Combo?
                                }

                                // Else we don't know, not that many rogues around these days

                                break;
                            }
                        case CClass.PALADIN:
                            {
                                if (_deckData.Cards.Any(c =>
                                  c == "AT_079")) // Mysterious Challenger
                                {
                                    _deckData.SubType = SubType.SecretPaladin;
                                    _deckData.ArchType = DeckArchtype.Aggro;
                                }
                                // Need more here
                                break;
                            }
                        case CClass.DRUID:
                            {
                                if (_deckData.Cards.Any(c =>
                                 c == "NEW1_026" || // Violet Teacher
                                 c == "EX1_160")) // Power of the Wild
                                {
                                    _deckData.SubType = SubType.TokenDruid;
                                    _deckData.ArchType = DeckArchtype.Combo;
                                }
                                else if (_deckData.Cards.Any(c =>
                                    c == "EX1_178")) // Ancient of War
                                {
                                    _deckData.SubType = SubType.RampDruid;
                                    _deckData.ArchType = DeckArchtype.Control;
                                }
                                else
                                {
                                    // Else it's most likely Mid Range Druid
                                    _deckData.SubType = SubType.MidRangeDruid;
                                    _deckData.ArchType = DeckArchtype.Midrange;
                                }
                                break;
                            }
                        case CClass.PRIEST:
                            {
                                if (_deckData.Cards.Any(c =>
                                    c == "AT_017" || // Twilight Guardian
                                    c == "BRM_033" || // Blackwing Technician
                                    c == "BRM_004" || // Twilight Whelp
                                    c == "AT_116" || // Wyrmrest Agent
                                    c == "BRM_034")) // Blackwing Corruptor
                                {
                                    _deckData.SubType = SubType.DragonPriest;
                                    _deckData.ArchType = DeckArchtype.Tempo;
                                }
                                else
                                {
                                    // Else it is most likely Control priest
                                    _deckData.SubType = SubType.ControlPriest;
                                    _deckData.ArchType = DeckArchtype.Control;
                                }
                                break;
                            }
                        case CClass.MAGE:
                            {
                                if (_deckData.Cards.Any(c =>
                                   c == "GVG_005" || // Echo of Medivh
                                   c == "EX1_620")) // Molten Giant
                                {
                                    _deckData.SubType = SubType.EchoMage;
                                    _deckData.ArchType = DeckArchtype.Control;
                                }
                                else if (_deckData.Cards.Any(c =>
                                   c == "GVG_003" || // Unstable Portal
                                   c == "BRM_002")) // Flame Walker
                                {
                                    _deckData.SubType = SubType.TempoMage;
                                    _deckData.ArchType = DeckArchtype.Tempo;
                                }
                                else if (_deckData.Cards.Any(c =>
                                   c == "EX1_295")) // Ice block
                                {
                                    _deckData.SubType = SubType.FreezeMage;
                                    _deckData.ArchType = DeckArchtype.Control;
                                }
                                else if (_deckData.Cards.Any(c =>
                                    c == "GVG_082" || // Clockwork Gnome
                                    c == "GVG_013" || // Cogmaster
                                    c == "GVG_085" || // Annoy-o-Tron
                                    c == "GVG_006")) // Mechwarper
                                {
                                    _deckData.SubType = SubType.MechMage;
                                    _deckData.ArchType = DeckArchtype.Aggro;
                                }
                                break;
                            }
                        case CClass.WARLOCK:
                            {
                                if (_deckData.Cards.Any(c =>
                                        (c == "GVG_021" ||// Mal'Ganis OR
                                         c == "FP1_022") && // Void Caller With one the following:
                                        (c == "EX1_043" || // Twilight Drake
                                        c == "EX1_620" || // Molten Giant
                                        c == "EX1_105"))) // Mountain Giant
                                {
                                    _deckData.SubType = SubType.DemonHandlock;
                                    _deckData.ArchType = DeckArchtype.Control;
                                }
                                else if (_deckData.Cards.Any(c =>
                                      c == "AT_017" || // Twilight Guardian
                                      c == "BRM_033" || // Blackwing Technician
                                      c == "BRM_034")) // Blackwing Corruptor
                                {
                                    _deckData.SubType = SubType.DragonHandlock;
                                    _deckData.ArchType = DeckArchtype.Control;
                                }
                                else if (_deckData.Cards.Any(c =>
                                      c == "EX1_043" || // Twilight Drake
                                      c == "EX1_620" || // Molten Giant
                                      c == "EX1_105")) // Mountain Giant
                                {
                                    _deckData.SubType = SubType.Handlock;
                                    _deckData.ArchType = DeckArchtype.Control;
                                }
                                else if (_deckData.Cards.Any(c =>
                                      c == "GVG_021" || // Mal'Ganis
                                      c == "EX1_620" || // Molten Giant
                                      c == "EX1_105")) // Mountain Giant
                                {
                                    _deckData.SubType = SubType.DemonZooWarlock;
                                    _deckData.ArchType = DeckArchtype.Aggro;
                                }
                                else
                                {
                                    // Else it's very likely to be zoolock
                                    _deckData.SubType = SubType.Zoolock;
                                    _deckData.ArchType = DeckArchtype.Aggro;
                                }
                                break;
                            }
                        case CClass.HUNTER:
                        {
                            if (_deckData.Cards.Any(c =>
                                     c == "GVG_043" || // Glaivezooka
                                     c == "CS2_188" || // Abusive Sergeant
                                     c == "EX1_089" || // Arcane Golem
                                     c == "EX1_029")) // Leper Gnome
                            {
                                _deckData.SubType = SubType.FaceHunter;
                                _deckData.ArchType = DeckArchtype.Aggro;
                            }
                            if (_deckData.Cards.Any(c =>
                                      c == "EX1_534" || // Savannah Highmane
                                      c == "EX1_620" || // Molten Giant
                                      c == "EX1_105")) // Mountain Giant
                            {
                                _deckData.SubType = SubType.MidRangeHunter;
                                _deckData.ArchType = DeckArchtype.Midrange;
                            }
                            break;
                        }
                        case CClass.SHAMAN:
                        {
                            if (_deckData.Cards.Any(c =>
                                    c == "AT_046" || // Tuskarr Totemic
                                    c == "AT_049")) // //Thunder Bluff Valiant
                            {
                                _deckData.SubType = SubType.TotemShaman;
                                _deckData.ArchType = DeckArchtype.Midrange;
                            }
                            else if (_deckData.Cards.Any(c =>
                                   c == "GVG_082" || // Clockwork Gnome
                                   c == "GVG_013" || // Cogmaster
                                   c == "GVG_085" || // Annoy-o-Tron
                                   c == "GVG_006")) // Mechwarper
                            {
                                _deckData.SubType = SubType.MechShaman;
                                _deckData.ArchType = DeckArchtype.Aggro;
                            }
                            if (_deckData.Cards.Any(c =>
                                c == "AT_017" || // Twilight Guardian
                                c == "BRM_033" || // Blackwing Technician
                                c == "BRM_034")) // Blackwing Corruptor
                            {
                                _deckData.SubType = SubType.DragonShaman;
                                _deckData.ArchType = DeckArchtype.Midrange;
                            }
                            else
                            {
                                // It's a random Control shaman
                                _deckData.SubType = SubType.ControlShaman;
                                _deckData.ArchType = DeckArchtype.Control;
                            }
                            break;
                        }
                    }
                }

                // If it's still null, it's unknown
                if (_deckData == null)
                {
                    _deckData = new DeckData
                    {
                        ArchType = DeckArchtype.Unknown,
                        SubType = SubType.Unknown,
                        Cards = new List<string>()
                    };
                }
                else
                {
                    // Save stats to database
                    SharedDataMgr.AddRow("OpponentDeckHits", new string[]
                    {
                        Bot.GetCurrentOpponentId().ToString(CultureInfo.InvariantCulture),
                        opponentClass.ToString(),
                        DateTime.Now.ToString("g"),
                        string.Join(",", _deckData.Cards),
                        _deckData.ArchType.ToString(),
                        _deckData.SubType.ToString()
                    });
                }
            }

            Bot.Log("[OpponentDeck] SubType: " + _deckData.SubType);
            Bot.Log("[OpponentDeck] Archtype: " + _deckData.ArchType);
            Bot.Log("[OpponentDeck] DeckList: " + string.Join(", ", _deckData.Cards));

            return _deckData;
        }

        private List<PlayerHistory> GetOpponentHistoryWithClass(CClass heroClass)
        {
            if (_playerHistories != null) return _playerHistories.OrderByDescending(h => h.Date).ToList();
            _playerHistories = new List<PlayerHistory>();

            SharedDataMgr.QueryRows(null, "OpponentDeckSaver", r =>
                r["OpponentId"] == Bot.GetCurrentOpponentId().ToString()
                && r["EnemyHero"] == heroClass.ToString());

            _messageReceived.WaitOne(3000);

            return _playerHistories.OrderByDescending(h => h.Date).ToList();
        }

        /// <summary>
        /// Gets called when data from QueryRows is received. Will save the data 
        /// </summary>
        /// <param name="data"></param>
        public void OnSharedDataReceived(Dictionary<int, Dictionary<string, string>> data)
        {
            foreach (var playerHistory in data.Keys.Select(key => new PlayerHistory(
                long.Parse(data[key]["OpponentId"]),
                (CClass)Enum.Parse(typeof(CClass), data[key]["EnemyHero"]),
                DateTime.Parse(data[key]["DateTime"]),
                data[key]["Deck"].Split(',').ToList()
                )))
            {
                _playerHistories.Add(playerHistory);
            }

            _messageReceived.Set();
        }

        #region Helper classes

        private class PlayerHistory
        {
            public PlayerHistory(long playerId, CClass enemyClass, DateTime date, List<string> cards)
            {
                this.PlayerId = playerId;
                this.EnemyClass = enemyClass;
                this.Date = date;
                this.Cards = cards;
            }

            public long PlayerId { get; set; }
            public CClass EnemyClass { get; set; }
            public DateTime Date { get; set; }
            public List<string> Cards { get; set; }
        }

        public class DeckData
        {
            public DeckArchtype ArchType { get; set; }
            public SubType SubType { get; set; }
            public List<string> Cards { get; set; }
        }

        public enum DeckArchtype
        {
            Unknown,
            Aggro,
            Control,
            Midrange,
            Combo,
            Tempo
        }

        public enum SubType
        {
            Unknown,
            PatronWarrior,
            ControlWarrior,
            SecretPaladin,
            MidRangeDruid,
            TokenDruid,
            DemonHandlock,
            DragonHandlock,
            TempoMage,
            DragonPriest,
            FreezeMage,
            MidRangePaladin,
            ControlPriest,
            DemonZooWarlock,
            MidRangeHunter,
            MechMage,
            Handlock,
            Zoolock,
            HybridHunter,
            EchoMage,
            AggroPaladin,
            DragonWarrior,
            FaceHunter,
            OilRogue,
            MechShaman,
            DragonShaman,
            TotemShaman,
            MalygosShaman,
            ControlShaman,
            MechWarrior,
            RampDruid
        }

        #endregion Helper classes

        #endregion OpponentDeck Stuff
    }
}
