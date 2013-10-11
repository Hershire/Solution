using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Library;
using Server.MirEnvir;
using System.Data;
using System.Data.SqlClient;
using S = Library.MirNetwork.ServerPackets;
using C = Library.MirNetwork.ClientPackets;
using Server.MirNetwork;
using System.Text.RegularExpressions;
using System.Threading;

namespace Server.MirDatabase
{
    static class MirDB
    {
        private static long StartTime;
        public static Thread DBThread;
        public static bool Saving;

        //Server Variables

        private static string ConnectionString;
        private static SqlConnection DBConnection;
        private static SqlCommand DBCommand;

        public static List<MapInfo> MapInfoList = new List<MapInfo>();
        public static List<ItemInfo> ItemInfoList = new List<ItemInfo>();
        public static List<MonsterInfo> MonsterInfoList = new List<MonsterInfo>();

        public static List<ItemInfo> StartItems = new List<ItemInfo>();
        public static List<SafeZoneInfo> StartPoints = new List<SafeZoneInfo>();

        public static ItemInfo GoldItem;


        //User Variables

        public static List<AccountInfo> AccountList = new List<AccountInfo>();
        private static Regex AccountIDReg, PasswordReg, EMailReg, CharacterReg;


        static MirDB()
        {
            StartTime = Main.Time;

            ConnectionString = string.Format("Server={0};Database={1};User ID={2};Password={3};",
                                       Settings.SQLServer, Settings.SQLDatabase, Settings.SQLUserID, Settings.SQLPassword);

            DBConnection = new SqlConnection(ConnectionString);
            DBCommand = DBConnection.CreateCommand();

            DBConnection.Open();

            LoadMapInfo();
            LoadSafeZoneInfo();
            LoadMovementInfo();
            LoadItemInfo();
            LoadMonsterInfo();
            //LoadMagicInfo();
            LoadRespawnInfo();
            LoadDropInfo();



            AccountIDReg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinAccountIDLength + "," + Globals.MaxAccountIDLength + "}$");
            PasswordReg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");
            EMailReg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            CharacterReg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinCharacterNameLength + "," + Globals.MaxCharacterNameLength + "}$");
        }

        private static void CleanUpItems()
        {
            DBCommand.CommandText = "DELETE FROM [Character Items] WHERE [Character Index] is NULL;";
            DBCommand.Parameters.Clear();
            DBCommand.ExecuteNonQuery();
        }
        public static void Start()
        {
            DBThread = new Thread(DBLoop);
            DBThread.IsBackground = true;
            DBThread.Start();
        }

        private static void DBLoop()
        {
            while (true)
            {
                Thread.Sleep(Settings.DBSaveTimer);

                Main.UpdateTime();
                Main.EnqueueMessage("Saving Database.");
                Save();
            }
        }

        public static void Stop()
        {
            CleanUpItems();

            if (DBConnection != null && DBConnection.State != ConnectionState.Closed)
                DBConnection.Close();
        }
        public static void Save()
        {
            Saving = true;

            Network.UpdateAllPlayers();
            
            for (int I = 0; I < AccountList.Count; I++)
                SaveAccount(AccountList[I]);

            SaveDropInfo();

            Saving = false;
        }

        static void SaveAccount(AccountInfo Account)
        {
            try
            {
                DBCommand.Parameters.Clear();
                DBCommand.CommandText =
                    "UPDATE [Account Info] SET" +
                    "[Account ID]=@AccountID, [Password]=@Password, " + 
                    "[Banned]=@Banned, [Ban Count]=@BanCount, [Ban Reason]=@BanReason, [Expiry Date]=@ExpiryDate, " + 
                    "[Gold]=@Gold WHERE [Account Index]=@AccountIndex";

                DBCommand.Parameters.AddWithValue("@AccountID", Account.AccountID);
                DBCommand.Parameters.AddWithValue("@Password", Account.Password);

                DBCommand.Parameters.AddWithValue("@Banned", Account.Banned);
                DBCommand.Parameters.AddWithValue("@BanCount", Account.BanCount);
                DBCommand.Parameters.AddWithValue("@BanReason", Account.BanReason);
                DBCommand.Parameters.AddWithValue("@ExpiryDate", Account.ExpiryDate).SqlDbType = SqlDbType.DateTime2;

                DBCommand.Parameters.AddWithValue("@Gold", Account.Gold);
                DBCommand.Parameters.AddWithValue("@AccountIndex", Account.AccountIndex);

                DBCommand.ExecuteNonQuery();

                for (int I = 0; I < Account.Characters.Count; I++)
                    SaveCharacter(Account.Characters[I], Account.AccountIndex);
            }
            catch (Exception Ex)
            {
                Main.EnqueueException(Ex);
            }
        }
        static void SaveCharacter(CharacterInfo Character, int AccountIndex)
        {
            DBCommand.Parameters.Clear();
            DBCommand.CommandText =
                "UPDATE [Character Info] SET" +
                "[Account Index]=@AccountIndex, [Character Name]=@CharacterName, [Level]=@Level, [Class]=@Class, [Gender]=@Gender, [Deleted]=@Deleted, " +
                "[Delete Count]=@DeleteCount, [Delete Date]=@DeleteDate, [Banned]=@Banned, [Ban Count]=@BanCount, [Ban Reason]=@BanReason, [Expiry Date]=@ExpiryDate, " +
                "[Last Access]=@LastAccess, [Direction]=@Direction, [Home Index]=@HomeIndex, [Home X]=@HomeX, [Home Y]=@HomeY, [Map Index]=@MapIndex," +
                "[Location X]=@LocationX, [Location Y]=@LocationY, [Hair Type]=@HairType, [Experience]=@Experience, [HP]=@HP, [MP]=@MP " +
                "WHERE [Character Index]=@CharacterIndex";

            DBCommand.Parameters.AddWithValue("@AccountIndex", AccountIndex);
            DBCommand.Parameters.AddWithValue("@CharacterName", Character.CharacterName);
            DBCommand.Parameters.AddWithValue("@Level", Character.Level);
            DBCommand.Parameters.AddWithValue("@Class", (int)Character.Class);
            DBCommand.Parameters.AddWithValue("@Gender", (int)Character.Gender);
            DBCommand.Parameters.AddWithValue("@Deleted", Character.Deleted);
            DBCommand.Parameters.AddWithValue("@DeleteCount", Character.DeleteCount);
            DBCommand.Parameters.AddWithValue("@DeleteDate", Character.DeleteDate).SqlDbType = SqlDbType.DateTime2;
            DBCommand.Parameters.AddWithValue("@Banned", Character.Banned);
            DBCommand.Parameters.AddWithValue("@BanCount", Character.BanCount);
            DBCommand.Parameters.AddWithValue("@BanReason", Character.BanReason);
            DBCommand.Parameters.AddWithValue("@ExpiryDate", Character.ExpiryDate).SqlDbType = SqlDbType.DateTime2;
            DBCommand.Parameters.AddWithValue("@LastAccess", Character.LastAccess).SqlDbType = SqlDbType.DateTime2;
            DBCommand.Parameters.AddWithValue("@Direction", (int)Character.Direction);
            DBCommand.Parameters.AddWithValue("@HomeIndex", Character.HomeIndex);
            DBCommand.Parameters.AddWithValue("@HomeX", Character.HomeLocation.X); 
            DBCommand.Parameters.AddWithValue("@HomeY", Character.HomeLocation.Y);
            DBCommand.Parameters.AddWithValue("@MapIndex", Character.MapIndex);
            DBCommand.Parameters.AddWithValue("@LocationX", Character.Location.X);
            DBCommand.Parameters.AddWithValue("@LocationY", Character.Location.Y);
            DBCommand.Parameters.AddWithValue("@HairType", Character.HairType);
            DBCommand.Parameters.AddWithValue("@Experience", Character.Experience);
            DBCommand.Parameters.AddWithValue("@HP", Character.HP);
            DBCommand.Parameters.AddWithValue("@MP", Character.MP);
            DBCommand.Parameters.AddWithValue("@CharacterIndex", Character.CharacterIndex);

            DBCommand.ExecuteNonQuery();

            SaveItems(Character);
            //SaveMagics(Character);
        }
        static void SaveItems(CharacterInfo Character)
        {
            DBCommand.CommandText = "UPDATE [Character Items] SET [Character Index]=NULL WHERE [Character Index]=@CharacterIndex;";
            DBCommand.Parameters.Clear();
            DBCommand.Parameters.AddWithValue("@CharacterIndex", Character.CharacterIndex);
            DBCommand.ExecuteNonQuery();

            DBCommand.CommandText =
                "UPDATE [Character Items] SET " +
                "[Character Index] =@CharacterIndex, [Slot Index]=@SlotIndex, [Current Durability]= @CurrentDurability, [Max Durability]=@MaxDurability, [Amount]=@Amount, " +
                "[Added AC]=@AddedAC,[Added MAC]=@AddedMAC, [Added DC]=@AddedDC, [Added MC]=@AddedMC, [Added SC]=@AddedSC, [Added Health]=@AddedHealth, " +
                "[Added Mana]=@AddedMana, [Added Accuracy]=@AddedAccuracy, [Added Agility]=@AddedAgility, [Added Body Weight]=@AddedBodyWeight, [Added Hand Weight]=@AddedHandWeight, " +
                "[Added Bag Weight]=@AddedBagWeight, [Added Luck]=@AddedLuck, [Added Attack Speed]=@AddedAttackSpeed, [Added Magic Resist]=@AddedMagicResist, " +
                "[Added Poison Resist]=@AddedPoisonResist, [Added Health Regen]=@AddedHealthRegen, [Added Mana Regen]=@AddedManaRegen " +
                "WHERE [Unique ID]=@UniqueID";

            UserItem Item;
            for (int I = 0; I < Character.Inventory.Length; I++)
            {
                Item = Character.Inventory[I];
                if (Item == null) continue;
                DBCommand.Parameters.Clear();
                DBCommand.Parameters.AddWithValue("@CharacterIndex", Character.CharacterIndex);
                DBCommand.Parameters.AddWithValue("@SlotIndex", I);
                DBCommand.Parameters.AddWithValue("@CurrentDurability", Item.CurrentDurability);
                DBCommand.Parameters.AddWithValue("@MaxDurability", Item.MaxDurability);
                DBCommand.Parameters.AddWithValue("@Amount", Item.Amount);
                DBCommand.Parameters.AddWithValue("@AddedAC", Item.AddedAC);
                DBCommand.Parameters.AddWithValue("@AddedMAC", Item.AddedMAC);
                DBCommand.Parameters.AddWithValue("@AddedDC", Item.AddedDC);
                DBCommand.Parameters.AddWithValue("@AddedMC", Item.AddedMC);
                DBCommand.Parameters.AddWithValue("@AddedSC", Item.AddedSC);
                DBCommand.Parameters.AddWithValue("@AddedHealth", Item.AddedHealth);
                DBCommand.Parameters.AddWithValue("@AddedMana", Item.AddedMana);
                DBCommand.Parameters.AddWithValue("@AddedAccuracy", Item.AddedAccuracy);
                DBCommand.Parameters.AddWithValue("@AddedAgility", Item.AddedAgility);
                DBCommand.Parameters.AddWithValue("@AddedBodyWeight", Item.AddedBodyWeight);
                DBCommand.Parameters.AddWithValue("@AddedHandWeight", Item.AddedHandWeight);
                DBCommand.Parameters.AddWithValue("@AddedBagWeight", Item.AddedBagWeight);
                DBCommand.Parameters.AddWithValue("@AddedLuck", Item.AddedLuck);
                DBCommand.Parameters.AddWithValue("@AddedAttackSpeed", Item.AddedAttackSpeed);
                DBCommand.Parameters.AddWithValue("@AddedMagicResist", Item.AddedMagicResist);
                DBCommand.Parameters.AddWithValue("@AddedPoisonResist", Item.AddedPoisonResist);
                DBCommand.Parameters.AddWithValue("@AddedHealthRegen", Item.AddedHealthRegen);
                DBCommand.Parameters.AddWithValue("@AddedManaRegen", Item.AddedManaRegen);
                DBCommand.Parameters.AddWithValue("@UniqueID", Item.UniqueID);
                DBCommand.ExecuteNonQuery();
            }
            for (int I = 0; I < Character.Equipment.Length; I++)
            {
                Item = Character.Equipment[I];
                if (Item == null) continue;
                DBCommand.Parameters.Clear();
                DBCommand.Parameters.AddWithValue("@CharacterIndex", Character.CharacterIndex);
                DBCommand.Parameters.AddWithValue("@SlotIndex", I + Character.Inventory.Length);
                DBCommand.Parameters.AddWithValue("@CurrentDurability", Item.CurrentDurability);
                DBCommand.Parameters.AddWithValue("@MaxDurability", Item.MaxDurability);
                DBCommand.Parameters.AddWithValue("@Amount", Item.Amount);
                DBCommand.Parameters.AddWithValue("@AddedAC", Item.AddedAC);
                DBCommand.Parameters.AddWithValue("@AddedMAC", Item.AddedMAC);
                DBCommand.Parameters.AddWithValue("@AddedDC", Item.AddedDC);
                DBCommand.Parameters.AddWithValue("@AddedMC", Item.AddedMC);
                DBCommand.Parameters.AddWithValue("@AddedSC", Item.AddedSC);
                DBCommand.Parameters.AddWithValue("@AddedHealth", Item.AddedHealth);
                DBCommand.Parameters.AddWithValue("@AddedMana", Item.AddedMana);
                DBCommand.Parameters.AddWithValue("@AddedAccuracy", Item.AddedAccuracy);
                DBCommand.Parameters.AddWithValue("@AddedAgility", Item.AddedAgility);
                DBCommand.Parameters.AddWithValue("@AddedBodyWeight", Item.AddedBodyWeight);
                DBCommand.Parameters.AddWithValue("@AddedHandWeight", Item.AddedHandWeight);
                DBCommand.Parameters.AddWithValue("@AddedBagWeight", Item.AddedBagWeight);
                DBCommand.Parameters.AddWithValue("@AddedLuck", Item.AddedLuck);
                DBCommand.Parameters.AddWithValue("@AddedAttackSpeed", Item.AddedAttackSpeed);
                DBCommand.Parameters.AddWithValue("@AddedMagicResist", Item.AddedMagicResist);
                DBCommand.Parameters.AddWithValue("@AddedPoisonResist", Item.AddedPoisonResist);
                DBCommand.Parameters.AddWithValue("@AddedHealthRegen", Item.AddedHealthRegen);
                DBCommand.Parameters.AddWithValue("@AddedManaRegen", Item.AddedManaRegen);
                DBCommand.Parameters.AddWithValue("@UniqueID", Item.UniqueID);
                DBCommand.ExecuteNonQuery();
            }
            for (int I = 0; I < Character.Storage.Length; I++)
            {
                Item = Character.Storage[I];
                if (Item == null) continue;
                DBCommand.Parameters.Clear();
                DBCommand.Parameters.AddWithValue("@CharacterIndex", Character.CharacterIndex);
                DBCommand.Parameters.AddWithValue("@SlotIndex", I + Character.Inventory.Length + Character.Equipment.Length);
                DBCommand.Parameters.AddWithValue("@CurrentDurability", Item.CurrentDurability);
                DBCommand.Parameters.AddWithValue("@MaxDurability", Item.MaxDurability);
                DBCommand.Parameters.AddWithValue("@Amount", Item.Amount);
                DBCommand.Parameters.AddWithValue("@AddedAC", Item.AddedAC);
                DBCommand.Parameters.AddWithValue("@AddedMAC", Item.AddedMAC);
                DBCommand.Parameters.AddWithValue("@AddedDC", Item.AddedDC);
                DBCommand.Parameters.AddWithValue("@AddedMC", Item.AddedMC);
                DBCommand.Parameters.AddWithValue("@AddedSC", Item.AddedSC);
                DBCommand.Parameters.AddWithValue("@AddedHealth", Item.AddedHealth);
                DBCommand.Parameters.AddWithValue("@AddedMana", Item.AddedMana);
                DBCommand.Parameters.AddWithValue("@AddedAccuracy", Item.AddedAccuracy);
                DBCommand.Parameters.AddWithValue("@AddedAgility", Item.AddedAgility);
                DBCommand.Parameters.AddWithValue("@AddedBodyWeight", Item.AddedBodyWeight);
                DBCommand.Parameters.AddWithValue("@AddedHandWeight", Item.AddedHandWeight);
                DBCommand.Parameters.AddWithValue("@AddedBagWeight", Item.AddedBagWeight);
                DBCommand.Parameters.AddWithValue("@AddedLuck", Item.AddedLuck);
                DBCommand.Parameters.AddWithValue("@AddedAttackSpeed", Item.AddedAttackSpeed);
                DBCommand.Parameters.AddWithValue("@AddedMagicResist", Item.AddedMagicResist);
                DBCommand.Parameters.AddWithValue("@AddedPoisonResist", Item.AddedPoisonResist);
                DBCommand.Parameters.AddWithValue("@AddedHealthRegen", Item.AddedHealthRegen);
                DBCommand.Parameters.AddWithValue("@AddedManaRegen", Item.AddedManaRegen);
                DBCommand.Parameters.AddWithValue("@UniqueID", Item.UniqueID);
                DBCommand.ExecuteNonQuery();
            }

        }



        static void SaveDropInfo()
        {
            MonsterInfo MI; DropInfo DI;
            DBCommand.CommandText = "UPDATE [Drop Info] SET [Total Count]=@TotalCount, [Trigger Count]=@TriggerCount WHERE ([Drop Index]=@DropIndex)";

            for (int M = 0; M < MonsterInfoList.Count; M++)
            {
                MI = MonsterInfoList[M];
                for (int D = 0; D < MI.DropList.Count; D++)
                {
                    DI = MI.DropList[D];
                    DBCommand.Parameters.Clear();
                    DBCommand.Parameters.AddWithValue("@TotalCount", DI.TotalCount);
                    DBCommand.Parameters.AddWithValue("@TriggerCount", DI.TriggerCount);
                    DBCommand.Parameters.AddWithValue("@DropIndex", DI.DropIndex);
                    DBCommand.ExecuteNonQuery();
                }
            }
        }



        /// Server Loading
        static void LoadMapInfo()
        {
            if (MapInfoList.Count > 0) return;

            DataSet DSet = new DataSet();

            DBCommand.CommandText = "SELECT * FROM [Map Info] WHERE ([Enabled] = 1);";
            SqlDataAdapter Adapter = new SqlDataAdapter(DBCommand);
            Adapter.Fill(DSet);
            DataTable DTable = DSet.Tables[0];

            for (int I = 0; I < DTable.Rows.Count; I++)
            {
                DataRow Row = DTable.Rows[I];
                MapInfoList.Add(new MapInfo(Row));
            }
        }
        static void LoadSafeZoneInfo()
        {
            for (int M = 0; M < MapInfoList.Count; M++)
            {
                MapInfo MInfo = MapInfoList[M];

                if (MInfo.SafeZoneList.Count > 0) continue;

                DataSet DSet = new DataSet();

                DBCommand.CommandText = string.Format("SELECT * FROM [SafeZone Info] WHERE (([Map Index] = {0}) AND ([Enabled] = 1));", MInfo.MapIndex);

                SqlDataAdapter Adapter = new SqlDataAdapter(DBCommand);
                Adapter.Fill(DSet);
                DataTable DTable = DSet.Tables[0];
                SafeZoneInfo SZI;
                for (int R = 0; R < DTable.Rows.Count; R++)
                {
                    SZI = new SafeZoneInfo(DTable.Rows[R]);
                    MInfo.SafeZoneList.Add(SZI);
                    if (SZI.StartPoint) StartPoints.Add(SZI);
                }
            }
        }
        static void LoadMovementInfo()
        {
            for (int M = 0; M < MapInfoList.Count; M++)
            {
                MapInfo MInfo = MapInfoList[M];

                if (MInfo.MovementList.Count > 0) continue;

                DataSet DSet = new DataSet();

                DBCommand.CommandText = string.Format("SELECT * FROM [Movement Info] WHERE (([Source Map] = {0}) AND ([Enabled] = 1));", MInfo.MapIndex);

                SqlDataAdapter Adapter = new SqlDataAdapter(DBCommand);
                Adapter.Fill(DSet);
                DataTable DTable = DSet.Tables[0];

                for (int R = 0; R < DTable.Rows.Count; R++)
                    MInfo.MovementList.Add(new MovementInfo(DTable.Rows[R]));
            }
        }
        static void LoadItemInfo()
        {
            if (ItemInfoList.Count > 0) return;

            DataSet DSet = new DataSet();

            DBCommand.CommandText = "SELECT * FROM [Item Info]";
            SqlDataAdapter Adapter = new SqlDataAdapter(DBCommand);
            Adapter.Fill(DSet);
            DataTable DTable = DSet.Tables[0];
            ItemInfo II;
            for (int I = 0; I < DTable.Rows.Count; I++)
            {
                II = new ItemInfo(DTable.Rows[I]);
                ItemInfoList.Add(II);
                if (II.StartItem) StartItems.Add(II);
            }
            GoldItem = ItemInfoList.FirstOrDefault(I => I.ItemName == "Gold");
        }
        static void LoadMonsterInfo()
        {
            if (MonsterInfoList.Count > 0) return;

            DataSet DSet = new DataSet();

            DBCommand.CommandText = "SELECT * FROM [Monster Info] WHERE ([Enabled] = 1)";
            SqlDataAdapter Adapter = new SqlDataAdapter(DBCommand);
            Adapter.Fill(DSet);
            DataTable DTable = DSet.Tables[0];

            for (int I = 0; I < DTable.Rows.Count; I++)
            {
                DataRow Row = DTable.Rows[I];
                MonsterInfoList.Add(new MonsterInfo(Row));
            }

        }
        static void LoadRespawnInfo()
        {
            for (int M = 0; M < MapInfoList.Count; M++)
            {
                MapInfo MInfo = MapInfoList[M];

                if (MInfo.RespawnList.Count > 0) continue;

                DataSet DSet = new DataSet();

                DBCommand.CommandText = string.Format("SELECT * FROM [Respawn Info] WHERE (([Map Index] = {0}) AND ([Enabled] = 1))", MInfo.MapIndex);

                SqlDataAdapter Adapter = new SqlDataAdapter(DBCommand);
                Adapter.Fill(DSet);
                DataTable DTable = DSet.Tables[0];
                int MonsterIndex;

                for (int R = 0; R < DTable.Rows.Count; R++)
                {
                    DataRow Row = DTable.Rows[R];
                    RespawnInfo RInfo = new RespawnInfo(Row) { MInfo = MInfo };

                    MonsterIndex = Row["Monster Index"] is DBNull ? -1 : (int)Row["Monster Index"];
                    RInfo.Monster = MonsterInfoList.FirstOrDefault(O => O.MonsterIndex == MonsterIndex);

                    if (RInfo.Monster != null)
                        MInfo.RespawnList.Add(RInfo);
                }

            }

        }
        static void LoadDropInfo()
        {
            for (int M = 0; M < MonsterInfoList.Count; M++)
            {
                MonsterInfo MInfo = MonsterInfoList[M];

                if (MInfo.DropList.Count > 0) continue;

                DataSet DSet = new DataSet();

                DBCommand.CommandText = string.Format("SELECT * FROM [Drop Info] WHERE (([Monster Index] = {0}) AND ([Enabled] = 1))", MInfo.MonsterIndex);

                SqlDataAdapter Adapter = new SqlDataAdapter(DBCommand);
                Adapter.Fill(DSet);
                DataTable DTable = DSet.Tables[0];
                int ItemIndex;

                for (int R = 0; R < DTable.Rows.Count; R++)
                {
                    DataRow Row = DTable.Rows[R];
                    DropInfo DInfo = new DropInfo(Row);

                    ItemIndex = Row["Item Index"] is DBNull ? -1 : (int)Row["Item Index"];
                    DInfo.Item = ItemInfoList.FirstOrDefault(O => O.ItemIndex == ItemIndex);

                    if (DInfo.Item != null && DInfo.Rate > 0)
                        MInfo.DropList.Add(DInfo);
                }
            }
        }


        public static UserItem NewItem(ItemInfo II)
        {
            DBCommand.Parameters.Clear();
            DBCommand.CommandText = "INSERT INTO [Character Items] ([Item Index]) " +
                                    "OUTPUT INSERTED.[Unique ID] " +
                                    "VALUES (@ItemIndex);";

            DBCommand.Parameters.AddWithValue("@ItemIndex", II.ItemIndex);

            int UniqueID = (int)DBCommand.ExecuteScalar();

            return new UserItem(II)
            {
                UniqueID = UniqueID,
                CurrentDurability = II.Durability,
                MaxDurability = II.Durability,
            };
        }

        /// User Loading

        static bool AccountExists(string AccountID)
        {
            DBCommand.Parameters.Clear();
            DBCommand.CommandText = "SELECT COUNT(*) FROM [Account Info] WHERE ([Account ID] = @AccountID)";
            DBCommand.Parameters.AddWithValue("@AccountID", AccountID);

            return (int)DBCommand.ExecuteScalar() > 0;
        }
        static AccountInfo GetAccount(string AccountID)
        {
            lock (AccountList)
            {
                AccountInfo TempAccount = AccountList.FirstOrDefault(A => string.Compare(A.AccountID, AccountID, true) == 0);

                if (TempAccount != null)
                    return TempAccount;

                if (AccountExists(AccountID))
                {
                    DataSet DSet = new DataSet();

                    DBCommand.Parameters.Clear();
                    DBCommand.CommandText = "SELECT * FROM [Account Info] WHERE [Account ID] = @AccountID";
                    DBCommand.Parameters.AddWithValue("@AccountID", AccountID);
                    SqlDataAdapter Adapter = new SqlDataAdapter(DBCommand);
                    Adapter.Fill(DSet);
                    DataTable DTable = DSet.Tables[0];

                    TempAccount = new AccountInfo(DTable.Rows[0]);

                    LoadCharacters(TempAccount);

                    AccountList.Add(TempAccount);
                }
                return TempAccount;

            }
        }

        static bool CharacterExists(string CharacterName)
        {
            DBCommand.Parameters.Clear();
            DBCommand.CommandText = "SELECT COUNT(*) FROM [Character Info] WHERE [Character Name] = @CharacterName";
            DBCommand.Parameters.AddWithValue("@CharacterName", CharacterName);

            return (int)DBCommand.ExecuteScalar() > 0;
        }
        static void LoadCharacters(AccountInfo Account)
        {
            DataSet DSet = new DataSet();

            DBCommand.Parameters.Clear();
            DBCommand.CommandText = "SELECT * FROM [Character Info] WHERE ([Account Index] = @AccountIndex)";
            DBCommand.Parameters.AddWithValue("@AccountIndex", Account.AccountIndex);
            SqlDataAdapter Adapter = new SqlDataAdapter(DBCommand);
            Adapter.Fill(DSet);
            DataTable DTable = DSet.Tables[0];

            CharacterInfo Character;
            DataRow Row;

            for (int I = 0; I < DTable.Rows.Count; I++)
            {
                Row = DTable.Rows[I];
                Character = new CharacterInfo(Row);
                LoadItems(Character);
                //LoadMagics(Character);
                Account.Characters.Add(Character);
            }

        }
        static void LoadItems(CharacterInfo Character)
        {
            DataSet DSet = new DataSet();

            DBCommand.Parameters.Clear();
            DBCommand.CommandText = "SELECT * FROM [Character Items] WHERE [Character Index] = @CharacterIndex";
            DBCommand.Parameters.AddWithValue("@CharacterIndex", Character.CharacterIndex);
            SqlDataAdapter Adapter = new SqlDataAdapter(DBCommand);
            Adapter.Fill(DSet);
            DataTable DTable = DSet.Tables[0];
            UserItem Item;
            DataRow Row;
            int Index = 0;

            for (int I = 0; I < DTable.Rows.Count; I++)
            {
                Row = DTable.Rows[I];
                Item = new UserItem(Row);

                Item.Info = MirDB.ItemInfoList.FirstOrDefault(O => O.ItemIndex == Item.ItemIndex);

                if (Item.Info != null)
                {
                    Index = Row["Slot Index"] is DBNull ? -1 : (int)Row["Slot Index"];

                    if (Index >= 0 && Index < Character.Inventory.Length)
                        Character.Inventory[Index] = Item;
                    else
                    {
                        Index -= Character.Inventory.Length;
                        if (Index >= 0 && Index < Character.Equipment.Length)
                            Character.Equipment[Index] = Item;
                        else
                        {
                            Index -= Character.Equipment.Length;
                            if (Index >= 0 && Index < Character.Storage.Length)
                                Character.Storage[Index] = Item;
                        }
                    }
                }
            }
        }
        static void NewAccount(C.NewAccount P, string IPAddress)
        {
            DBCommand.Parameters.Clear();
            DBCommand.CommandText =
                "INSERT INTO [Account Info] (" +
                "[Account ID], [Password], [Birth Date], [User Name], [Secret Question], [Secret Answer], [EMail Address], [Creator IP], [Creation Date])" +
                "VALUES (@AccountID, @Password, @BirthDate, @UserName, @SecretQuestion, @SecretAnswer, @EMailAddress, @CreatorIP, @CreationDate)";

            DBCommand.Parameters.AddWithValue("@AccountID", P.AccountID);
            DBCommand.Parameters.AddWithValue("@Password", P.Password);
            DBCommand.Parameters.AddWithValue("@BirthDate", P.BirthDate.Ticks);
            DBCommand.Parameters.AddWithValue("@UserName", P.UserName);
            DBCommand.Parameters.AddWithValue("@SecretQuestion", P.SecretQuestion);
            DBCommand.Parameters.AddWithValue("@SecretAnswer", P.SecretAnswer);
            DBCommand.Parameters.AddWithValue("@EMailAddress", P.EMailAddress);

            DBCommand.Parameters.AddWithValue("@CreatorIP", IPAddress);
            DBCommand.Parameters.AddWithValue("@CreationDate", Main.Now.Ticks);

            DBCommand.ExecuteNonQuery();
        }
        

        public static void CreateAccount(C.NewAccount P, MirConnection Con)
        {
            if (!Settings.AllowNewAccount)
            {
                Con.QueuePacket(new S.NewAccount { Result = 1 });
                return;
            }

            if (!AccountIDReg.IsMatch(P.AccountID))
            {
                Con.QueuePacket(new S.NewAccount { Result = 2 });
                return;
            }

            if (!PasswordReg.IsMatch(P.Password))
            {
                Con.QueuePacket(new S.NewAccount { Result = 3 });
                return;
            }
            if (!string.IsNullOrWhiteSpace(P.EMailAddress) && !EMailReg.IsMatch(P.EMailAddress))
            {
                Con.QueuePacket(new S.NewAccount { Result = 4 });
                return;
            }

            if (!string.IsNullOrWhiteSpace(P.UserName) && P.UserName.Length > 30)
            {
                Con.QueuePacket(new S.NewAccount { Result = 5 });
                return;
            }

            if (!string.IsNullOrWhiteSpace(P.SecretQuestion) && P.SecretQuestion.Length > 20)
            {
                Con.QueuePacket(new S.NewAccount { Result = 6 });
                return;
            }

            if (!string.IsNullOrWhiteSpace(P.SecretAnswer) && P.SecretAnswer.Length > 20)
            {
                Con.QueuePacket(new S.NewAccount { Result = 7 });
                return;
            }

            if (AccountExists(P.AccountID))
            {
                Con.QueuePacket(new S.NewAccount { Result = 8 });
                return;
            }

            NewAccount(P, Con.IPAddress);

            Con.QueuePacket(new S.NewAccount { Result = 9 });
        }
        public static void ChangePassword(C.ChangePassword P, MirConnection Con)
        {
            if (!Settings.AllowChangePassword)
            {
                Con.QueuePacket(new S.ChangePassword { Result = 1 });
                return;
            }

            if (!AccountIDReg.IsMatch(P.AccountID))
            {
                Con.QueuePacket(new S.ChangePassword { Result = 2 });
                return;
            }

            if (!PasswordReg.IsMatch(P.CurrentPassword))
            {
                Con.QueuePacket(new S.ChangePassword { Result = 3 });
                return;
            }

            if (!PasswordReg.IsMatch(P.NewPassword))
            {
                Con.QueuePacket(new S.ChangePassword { Result = 4 });
                return;
            }


            AccountInfo TempAccount = GetAccount(P.AccountID);

            if (TempAccount == null)
            {
                Con.QueuePacket(new S.ChangePassword { Result = 5 });
                return;
            }

            if (TempAccount.Banned)
            {
                if (TempAccount.ExpiryDate > Main.Now)
                {
                    Con.QueuePacket(new S.ChangePasswordBanned { Reason = TempAccount.BanReason, ExpiryDate = TempAccount.ExpiryDate });
                    return;
                }
                else
                {
                    TempAccount.Banned = false;
                    TempAccount.BanReason = string.Empty;
                    TempAccount.ExpiryDate = DateTime.MinValue;
                }
            }

            if (string.Compare(TempAccount.Password, P.CurrentPassword, false) != 0)
            {
                Con.QueuePacket(new S.ChangePassword { Result = 6 });
                return;
            }

            TempAccount.Password = P.NewPassword;
            Con.QueuePacket(new S.ChangePassword { Result = 7 });

        }
        public static void Login(C.Login P, MirConnection Con)
        {
            if (!Settings.AllowLogin)
            {
                Con.QueuePacket(new S.Login { Result = 1 });
                return;
            }

            if (!AccountIDReg.IsMatch(P.AccountID))
            {
                Con.QueuePacket(new S.Login { Result = 2 });
                return;
            }

            if (!PasswordReg.IsMatch(P.Password))
            {
                Con.QueuePacket(new S.Login { Result = 3 });
                return;
            }


            AccountInfo TempAccount = GetAccount(P.AccountID);

            if (TempAccount == null)
            {
                Con.QueuePacket(new S.Login { Result = 4 });
                return;
            }

            if (TempAccount.Banned)
            {
                if (TempAccount.ExpiryDate > DateTime.Now)
                {
                    Con.QueuePacket(new S.LoginBanned { Reason = TempAccount.BanReason, ExpiryDate = TempAccount.ExpiryDate });
                    return;
                }
                else
                {
                    TempAccount.Banned = false;
                    TempAccount.BanReason = string.Empty;
                    TempAccount.ExpiryDate = DateTime.MinValue;
                }
            }

            if (string.Compare(TempAccount.Password, P.Password, false) != 0)
            {
                Con.QueuePacket(new S.Login { Result = 5 });
                return;
            }

            Network.Disconnect(TempAccount, 1);

            Con.QueuePacket(new S.LoginSuccess { CharacterList = TempAccount.GetSelectInfo() });

            Con.Account = TempAccount;
            Con.Stage = GameStage.Select;
        }
        public static CharacterInfo NewCharacter(C.NewCharacter P, MirConnection Con)
        {
            DateTime CreationDate = Main.Now;
            DBCommand.Parameters.Clear();
            DBCommand.CommandText =
                "INSERT INTO [Character Info] ([Account Index], [Character Name], [Class], [Gender], [Creator IP], [Creation Date], [Last Access]) " +
                "OUTPUT INSERTED.[Character Index] " +
                "VALUES (@AccountIndex, @CharacterName, @Class, @Gender, @CreatorIP, @CreationDate, @LastAccess)";

            DBCommand.Parameters.AddWithValue("@AccountIndex", Con.Account.AccountIndex);
            DBCommand.Parameters.AddWithValue("@CharacterName", P.CharacterName);
            DBCommand.Parameters.AddWithValue("@Class", (byte)P.Class);
            DBCommand.Parameters.AddWithValue("@Gender", (byte)P.Gender);

            DBCommand.Parameters.AddWithValue("@CreatorIP", Con.IPAddress);
            DBCommand.Parameters.AddWithValue("@CreationDate", CreationDate.Ticks);
            DBCommand.Parameters.AddWithValue("@LastAccess", 0);

            int Index =(int) DBCommand.ExecuteScalar();

            return new CharacterInfo(P) { CharacterIndex = Index, CreationDate = CreationDate, LastAccess = DateTime.MinValue };
        }
        public static void CreateCharacter(C.NewCharacter P, MirConnection Con)
        {
            if (!Settings.AllowNewCharacter)
            {
                Con.QueuePacket(new S.NewCharacter { Result = 1 });
                return;
            }

            if (!AccountIDReg.IsMatch(P.CharacterName))
            {
                Con.QueuePacket(new S.NewCharacter { Result = 2 });
                return;
            }

            if (P.Gender != MirGender.Male && P.Gender != MirGender.Female)
            {
                Con.QueuePacket(new S.NewCharacter { Result = 3 });
                return;
            }

            if (P.Class != MirClass.Warrior && P.Class != MirClass.Wizard && P.Class != MirClass.Taoist &&
                P.Class != MirClass.Assassin)
            {
                Con.QueuePacket(new S.NewCharacter { Result = 4 });
                return;
            }

            int CharCount = Con.Account.Characters.Count(C => !C.Deleted);

            if (CharCount >= Globals.MaxCharacterCount)
            {
                Con.QueuePacket(new S.NewCharacter { Result = 5 });
                return;
            }

            if (CharacterExists(P.CharacterName))
            {
                Con.QueuePacket(new S.NewCharacter { Result = 6 });
                return;
            }

            CharacterInfo CI = NewCharacter(P, Con);

            Con.Account.Characters.Add(CI);

            Con.QueuePacket(new S.NewCharacterSuccess { CharInfo = CI.ToSelectInfo() });
        }

    }

}
