using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.MirGraphics;
using Library;
using System.Drawing;
using Client.MirObjects;
using Client.MirScenes.Game_Scene;
using System.Windows.Forms;
using Client.MirNetwork;
using Client.MirSound;

namespace Client.MirControls
{
    class MirItemCell : MirImageControl
    {
        public static MirItemCell SelectedCell;
        public static bool PickedUpGold;

        internal UserItem Item
        {
            get { return ItemArray == null ? null : ItemArray[_ItemSlot]; }
            set
            {
                if (ItemArray != null)
                    ItemArray[_ItemSlot] = value;
            }
        }
        internal UserItem[] ItemArray
        {
            get
            {
                switch (GridType)
                {
                    case MirGridType.Inventory:
                        return MapObject.User.Inventory;
                    case MirGridType.Equipment:
                        return MapObject.User.Equipment;
                    default:
                        throw new NotImplementedException();
                }

            }
        }

        public override bool Border
        {
            get { return SelectedCell == this || MouseControl == this; }
        }

        #region GridType
        protected MirGridType _GridType;
        public event EventHandler GridTypeChanged;
        public MirGridType GridType
        {
            get { return _GridType; }
            set
            {
                if (_GridType == value) return;
                _GridType = value;
                OnGridTypeChanged();
            }
        }
        protected virtual void OnGridTypeChanged()
        {
            switch (GridType)
            {
                case MirGridType.Equipment:
                case MirGridType.Inventory:
                    Library = Libraries.Items;
                    break;
                default:
                    Library = null;
                    break;
            }
            if (GridTypeChanged != null)
                GridTypeChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region ItemSlot

        private int _ItemSlot;
        public event EventHandler ItemSlotChanged;
        public int ItemSlot
        {
            get { return _ItemSlot; }
            set
            {
                if (_ItemSlot == value) return;
                _ItemSlot = value;
                OnItemSlotChanged();
            }
        }
        protected virtual void OnItemSlotChanged()
        {
            if (ItemSlotChanged != null)
                ItemSlotChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Count Label
        protected MirLabel CountLabel { get; private set; }
        #endregion
        
        public MirItemCell()
        {
            Size = new Size(36, 32);
            GridType = MirGridType.None;
            DrawImage = false;

            BorderColor = Color.Lime;

            BackColor = Color.FromArgb(75, 255, 125, 125);
            DrawControlTexture = true;
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            if (PickedUpGold) return;

            base.OnMouseClick(e);

            if (Item != null && SelectedCell == null)
                PlayItemSound();
            

            if (e.Button == MouseButtons.Right)
            {
                if (Main.Time > Main.UseItemTime)
                    UseItem();
            }
            else
                MoveItem();
        }

        private void PlayItemSound()
        {
            if (Item == null) return;

            switch (Item.Info.ItemType)
            {
                case MirItemType.Weapon:
                    SoundManager.PlaySound(SoundList.ClickWeapon, false);
                    break;
                case MirItemType.ArmourFemale:
                case MirItemType.ArmourMale:
                    SoundManager.PlaySound(SoundList.ClickArmour, false);
                    break;
                case MirItemType.Helmet:
                    SoundManager.PlaySound(SoundList.ClickHelemt, false);
                    break;
                case MirItemType.Necklace:
                    SoundManager.PlaySound(SoundList.ClickNecklace, false);
                    break;
                case MirItemType.Bracelet:
                    SoundManager.PlaySound(SoundList.ClickBracelet, false);
                    break;
                case MirItemType.Ring:
                    SoundManager.PlaySound(SoundList.ClickRing, false);
                    break;
                case MirItemType.Boots:
                    SoundManager.PlaySound(SoundList.ClickBoots, false);
                    break;
                case MirItemType.Potion:
                    SoundManager.PlaySound(SoundList.ClickDrug, false);
                    break;
                default:
                    SoundManager.PlaySound(SoundList.ClickItem, false);
                    break;
            }
        }
        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (((MouseEventArgs)e).Button == MouseButtons.Right) return;

            UseItem();
        }
        public void UseItem()
        {
            if (Item == null) return;

            UserItem Temp;
            switch (Item.Info.ItemType)
            {
                case MirItemType.Weapon:
                    if (CharacterDialog.WeaponCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.WeaponCell.ItemSlot);
                        Temp = CharacterDialog.WeaponCell.Item;
                        CharacterDialog.WeaponCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    break;
                case MirItemType.ArmourMale:
                case MirItemType.ArmourFemale:
                    if (CharacterDialog.ArmorCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.ArmorCell.ItemSlot);
                        Temp = CharacterDialog.ArmorCell.Item;
                        CharacterDialog.ArmorCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    break;
                case MirItemType.Helmet:
                    if (CharacterDialog.HelmetCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.HelmetCell.ItemSlot);
                        Temp = CharacterDialog.HelmetCell.Item;
                        CharacterDialog.HelmetCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    return;
                case MirItemType.Necklace:
                    if (CharacterDialog.NecklaceCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.NecklaceCell.ItemSlot);
                        Temp = CharacterDialog.NecklaceCell.Item;
                        CharacterDialog.NecklaceCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    break;
                case MirItemType.Bracelet:
                    if (CharacterDialog.BraceletRCell.Item == null && CharacterDialog.BraceletRCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.BraceletRCell.ItemSlot);
                        Temp = CharacterDialog.BraceletRCell.Item;
                        CharacterDialog.BraceletRCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    else if (CharacterDialog.BraceletLCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.BraceletLCell.ItemSlot);
                        Temp = CharacterDialog.BraceletLCell.Item;
                        CharacterDialog.BraceletLCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    break;
                case MirItemType.Ring:
                    if (CharacterDialog.RingRCell.Item == null && CharacterDialog.RingRCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.RingRCell.ItemSlot);
                        Temp = CharacterDialog.RingRCell.Item;
                        CharacterDialog.RingRCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    else if (CharacterDialog.RingLCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.RingLCell.ItemSlot);
                        Temp = CharacterDialog.RingLCell.Item;
                        CharacterDialog.RingLCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    break;
                case MirItemType.Amulet:
                    if (CharacterDialog.AmuletCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.AmuletCell.ItemSlot);
                        Temp = CharacterDialog.AmuletCell.Item;
                        CharacterDialog.AmuletCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    break;
                case MirItemType.Belt:
                    if (CharacterDialog.BeltCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.BeltCell.ItemSlot);
                        Temp = CharacterDialog.BeltCell.Item;
                        CharacterDialog.BeltCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    break;
                case MirItemType.Boots:
                    if (CharacterDialog.BootsCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.BootsCell.ItemSlot);
                        Temp = CharacterDialog.BootsCell.Item;
                        CharacterDialog.BootsCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    break;
                case MirItemType.Stone:
                    if (CharacterDialog.StoneCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.StoneCell.ItemSlot);
                        Temp = CharacterDialog.StoneCell.Item;
                        CharacterDialog.StoneCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    break;
                case MirItemType.Torch:
                    if (CharacterDialog.TorchCell.CanWearItem(Item))
                    {
                        OutBound.EquipItem(GridType, Item.UniqueID, CharacterDialog.TorchCell.ItemSlot);
                        Temp = CharacterDialog.TorchCell.Item;
                        CharacterDialog.TorchCell.Item = Item;
                        Item = Temp;
                        MapObject.User.RefreshLibraries();
                    }
                    break;
                case MirItemType.Potion:
                case MirItemType.Scroll:
                    if (CanUseItem() && GridType == MirGridType.Inventory)
                    {
                        OutBound.UseItem(GridType, Item.UniqueID);

                        if (Item.Amount > 1) Item.Amount--;
                        else Item = null;
                    }
                    break;
                case MirItemType.Tiger:
                    break;
            }

            PlayItemSound();
            MapObject.User.CalculateWeight();
        }

        private void MoveItem()
        {
            if (SelectedCell == this)
            {
                SelectedCell = null;
                return;
            }

            UserItem I;
            if (SelectedCell != null)
            {
                if (SelectedCell.Item == null)
                {
                    SelectedCell = null;
                    return;
                }

                switch (GridType)
                {
                    #region To Inventory
                    case MirGridType.Inventory:
                        switch (SelectedCell.GridType)
                        {
                            case MirGridType.Inventory: //From Inventory
                                OutBound.MoveItem(GridType, SelectedCell.ItemSlot, ItemSlot);
                                if (Item == null)
                                {
                                    Item = SelectedCell.Item;
                                    SelectedCell.Item = null;
                                    SelectedCell = null;
                                    return;
                                }
                                I = Item;
                                Item = SelectedCell.Item;
                                SelectedCell.Item = I;
                                return;
                            case MirGridType.Equipment: // From Equipment
                                if (!CanRemoveItem(SelectedCell.Item))
                                {
                                    SelectedCell = null;
                                    return;
                                }

                                OutBound.RemoveItem(GridType, SelectedCell.Item.UniqueID, ItemSlot);

                                if (ItemArray[ItemSlot] == null)
                                {
                                    ItemArray[ItemSlot] = SelectedCell.Item;
                                    SelectedCell.Item = null;
                                    SelectedCell = null;
                                    MapObject.User.RefreshLibraries();
                                    MapObject.User.CalculateWeight();
                                    return;
                                }

                                for (int X = 0; X < ItemArray.Length; X++)
                                    if (ItemArray[X] == null)
                                    {
                                        ItemArray[X] = SelectedCell.Item;
                                        SelectedCell.Item = null;
                                        SelectedCell = null;
                                        MapObject.User.RefreshLibraries();
                                        MapObject.User.CalculateWeight();
                                        return;
                                    }
                                break;
                        }
                        break;
                    #endregion
                    #region To Equipment
                    case MirGridType.Equipment:
                        {
                            if (CorrectSlot(SelectedCell.Item))
                            {
                                if (CanWearItem(SelectedCell.Item))
                                {
                                    OutBound.EquipItem(SelectedCell.GridType, SelectedCell.Item.UniqueID, ItemSlot);
                                    I = Item;
                                    Item = SelectedCell.Item;
                                    SelectedCell.Item = I;
                                }
                                SelectedCell = null;
                                MapObject.User.RefreshLibraries();
                                MapObject.User.CalculateWeight();
                            }
                            return;
                        }
                    #endregion
                }

                return;
            }
            
            if (Item != null)
                SelectedCell = this;
        }

        private bool CanRemoveItem(UserItem I)
        {
            return ItemArray.Count(O => O == null) > 0;
        }

        public bool CorrectSlot(UserItem I)
        {
            MirItemType Type = I.Info.ItemType;

            switch ((MirEquipmentSlot)ItemSlot)
            {
                case MirEquipmentSlot.Weapon:
                    return Type == MirItemType.Weapon;
                case MirEquipmentSlot.Armour:
                    return Type == MirItemType.ArmourMale || Type == MirItemType.ArmourFemale;
                case MirEquipmentSlot.Helmet:
                    return Type == MirItemType.Helmet;
                case MirEquipmentSlot.Torch:
                    return Type == MirItemType.Torch;
                case MirEquipmentSlot.Necklace:
                    return Type == MirItemType.Necklace;
                case MirEquipmentSlot.BraceletL:
                case MirEquipmentSlot.BraceletR:
                    return Type == MirItemType.Bracelet;
                case MirEquipmentSlot.RingL:
                case MirEquipmentSlot.RingR:
                    return Type == MirItemType.Ring;
                case MirEquipmentSlot.Amulet:
                    return Type == MirItemType.Amulet;
                case MirEquipmentSlot.Boots:
                    return Type == MirItemType.Boots;
                case MirEquipmentSlot.Belt:
                    return Type == MirItemType.Belt;
                case MirEquipmentSlot.Stone:
                    return Type == MirItemType.Stone;
                case MirEquipmentSlot.Tiger:
                    return Type == MirItemType.Tiger;
                default:
                    return false;
            }

        }
        private bool CanUseItem()
        {
            if (Item == null) return false;

            switch (MapObject.User.Class)
            {
                case MirClass.Warrior:
                    if (!Item.Info.RequiredClass.HasFlag(MirRequiredClass.Warrior))
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "Warriors cannot use this item.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirClass.Wizard:
                    if (!Item.Info.RequiredClass.HasFlag(MirRequiredClass.Wizard))
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "Wizard cannot use this item.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirClass.Taoist:
                    if (!Item.Info.RequiredClass.HasFlag(MirRequiredClass.Taoist))
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "Taoist cannot use this item.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirClass.Assassin:
                    if (!Item.Info.RequiredClass.HasFlag(MirRequiredClass.Assassin))
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "Assassin cannot use this item.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
            }

            switch (Item.Info.RequiredType)
            {
                case MirRequiredType.Level:
                    if (MapObject.User.Level < Item.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You are not a high enough level.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirRequiredType.AC:
                    if (MapObject.User.MaxAC < Item.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You do not have enough AC.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirRequiredType.MAC:
                    if (MapObject.User.MaxMAC < Item.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You do not have enough MAC.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirRequiredType.DC:
                    if (MapObject.User.MaxMAC < Item.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You do not have enough DC.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirRequiredType.MC:
                    if (MapObject.User.MaxMAC < Item.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You do not have enough MC.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirRequiredType.SC:
                    if (MapObject.User.MaxMAC < Item.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You do not have enough SC.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
            }
            return true;
        }

        private bool CanWearItem(UserItem I)
        {
            if (I == null) return false;

            //If Can remove;

            if (I.Info.ItemType == MirItemType.ArmourMale)
            {
                if (MapObject.User.Gender != MirGender.Male)
                {
                    ChatPanel.RecieveChat(new ChatInfo { Message = "You are not Male.", Type = MirChatType.RedSystem });
                    return false;
                }
            }
            else if (I.Info.ItemType == MirItemType.ArmourFemale)
            {
                if (MapObject.User.Gender != MirGender.Female)
                {
                    ChatPanel.RecieveChat(new ChatInfo { Message = "You are not Female.", Type = MirChatType.RedSystem });
                    return false;
                }
            }

            switch (MapObject.User.Class)
            {
                case MirClass.Warrior:
                    if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Warrior))
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "Warriors cannot use this item.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirClass.Wizard:
                    if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Wizard))
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "Wizard cannot use this item.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirClass.Taoist:
                    if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Taoist))
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "Taoist cannot use this item.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirClass.Assassin:
                    if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Assassin))
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "Assassin cannot use this item.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
            }

            switch (I.Info.RequiredType)
            {
                case MirRequiredType.Level:
                    if (MapObject.User.Level < I.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You are not a high enough level.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirRequiredType.AC:
                    if (MapObject.User.MaxAC < I.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You do not have enough AC.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirRequiredType.MAC:
                    if (MapObject.User.MaxMAC < I.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You do not have enough MAC.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirRequiredType.DC:
                    if (MapObject.User.MaxMAC < I.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You do not have enough DC.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirRequiredType.MC:
                    if (MapObject.User.MaxMAC < I.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You do not have enough MC.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
                case MirRequiredType.SC:
                    if (MapObject.User.MaxMAC < I.Info.RequiredAmount)
                    {
                        ChatPanel.RecieveChat(new ChatInfo { Message = "You do not have enough SC.", Type = MirChatType.RedSystem });
                        return false;
                    }
                    break;
            }

            if (I.Info.ItemType == MirItemType.Weapon || I.Info.ItemType == MirItemType.Torch)
            {
                if (I.Info.Weight - (Item != null ? Item.Info.Weight : 0) + MapObject.User.CurrentHandWeight > MapObject.User.MaxHandWeight)
                {
                    ChatPanel.RecieveChat(new ChatInfo { Message = "It is too heavy to Hold.", Type = MirChatType.RedSystem });
                    return false;
                }
            }
            else
            {
                if (I.Info.Weight - (Item != null ? Item.Info.Weight : 0) + MapObject.User.CurrentBodyWeight > MapObject.User.MaxBodyWeight)
                {
                    ChatPanel.RecieveChat(new ChatInfo { Message = "It is too heavy to wear.", Type = MirChatType.RedSystem });
                    return false;
                }
            }

            return true;
        }

        protected override void DrawControl()
        {
            if (SelectedCell == this)
                base.DrawControl();

            if (Item != null && SelectedCell != this)
            {
                CreateDisposeLabel();

                if (Library != null)
                {
                    Size ImgSize = Library.GetSize(Item.Info.Image);
                    Point OffSet = new Point((Size.Width - ImgSize.Width) / 2, (Size.Height - ImgSize.Height) / 2);
                    Library.Draw(Item.Info.Image, Functions.PointA(DisplayLocation, OffSet), _ForeColor, _Opacity, UseOffSet);
                }

            }
            else
                DisposeCountLabel();
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
            GameScene.CreateItemLabel(Item, Main.PointToC(Cursor.Position));

        }
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
            GameScene.DisposeItemLabel();
            
        }


        private void CreateDisposeLabel()
        {
            if (Item.Info.StackSize < 1)
            {
                DisposeCountLabel();
                return;
            }

            if (CountLabel == null || CountLabel.IsDisposed)
            {
                CountLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = Color.Yellow,
                    NotControl = true,
                    OutLine = false,
                    Parent = this,
                };
            }

            CountLabel.Text = Item.Amount.ToString("###0");
            CountLabel.Location = new Point(Size.Width - CountLabel.Size.Width, Size.Height - CountLabel.Size.Height);
        }

        private void DisposeCountLabel()
        {
            if (CountLabel != null && !CountLabel.IsDisposed)
                CountLabel.Dispose();
            CountLabel = null;
        }
    }

}
