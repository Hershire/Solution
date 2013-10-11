using System;
using Client.MirScenes;
using Client.MirScenes.Login_Scene;
using Client.MirScenes.Select_Scene;
using Library;
using Library.MirNetwork;
using Library.MirNetwork.ServerPackets;
using Client.MirControls;
using Client.MirScenes.Game_Scene;
using Client.MirObjects;
using Client.MirSound;

namespace Client.MirNetwork
{
    static class InBound
    {
        public static void ProcessPacket(Packet P)
        {
            if (P == null) return;

            try
            {
                typeof(InBound).GetMethod(P.GetType().Name).Invoke(null, new object[] { P });
            }
            catch (Exception Ex)
            {
                if (Settings.LogErrors) Main.SaveError("Packet, " + P.GetType().Name + ": " + Ex);
            }

        }

        public static void ClientVersion(ClientVersion P)
        {
            switch (P.Result)
            {
                case 0:
                    SceneFunctions.WrongVersion();
                    Network.Disconnect();
                    break;
                case 1:
                    LoginScene.Connected();
                    break;
#if DEBUG
                default:
                    throw new NotImplementedException();
#endif
            }
        }
        public static void Disconnect(Disconnect P)
        {
            Network.Disconnect();

            SceneFunctions.ConClosed = true; 
            MirMessageBox MMBox;

            switch (P.Reason)
            {
                case 0:
                    MMBox = new MirMessageBox("You have been disconnected from the game.");
                    MMBox.OKButton.Click += (o, e) => SceneFunctions.QuitGame();
                    MMBox.Show();
                    break;
                case 1:
                    MMBox = new MirMessageBox("You have been disconnected from the game,\nAnother user logged onto your account.");
                    MMBox.OKButton.Click += (o, e) => SceneFunctions.QuitGame();
                    MMBox.Show();
                    break;
#if DEBUG
                default:
                    throw new NotImplementedException();
#endif
            }
        }
        public static void NewAccount(NewAccount P)
        {
            NewAccountDialog.ConfirmButton.Enabled = true;
            switch (P.Result)
            {
                case 0:
                    SceneFunctions.ShowMessage("An error occured whilst creating the account.");
                    NewAccountDialog.Hide();
                    NewAccountDialog.Clear();
                    LoginDialog.Show();
                    break;
                case 1:
                    SceneFunctions.ShowMessage("Account creation is currently disabled.");
                    NewAccountDialog.Hide();
                    NewAccountDialog.Clear();
                    LoginDialog.Show();
                    break;
                case 2:
                    SceneFunctions.ShowMessage("Your AccountID is not acceptable.");
                    NewAccountDialog.AccountIDTextBox.SetFocus();
                    break;
                case 3:
                    SceneFunctions.ShowMessage("Your Password is not acceptable.");
                    NewAccountDialog.Password1TextBox.SetFocus();
                    break;
                case 4:
                    SceneFunctions.ShowMessage("Your E-Mail Address is not acceptable.");
                    NewAccountDialog.EMailTextBox.SetFocus();
                    break;
                case 5:
                    SceneFunctions.ShowMessage("Your User Name is not acceptable.");
                    NewAccountDialog.UserNameTextBox.SetFocus();
                    break;
                case 6:
                    SceneFunctions.ShowMessage("Your Secret Question is not acceptable.");
                    NewAccountDialog.QuestionTextBox.SetFocus();
                    break;
                case 7:
                    SceneFunctions.ShowMessage("Your Secret Answer is not acceptable.");
                    NewAccountDialog.AnswerTextBox.SetFocus();
                    break;
                case 8:
                    SceneFunctions.ShowMessage("An Account with this ID already exists.");
                    NewAccountDialog.AccountIDTextBox.SetFocus();
                    NewAccountDialog.AccountIDTextBox.Text = string.Empty;
                    break;
                case 9:
                    SceneFunctions.ShowMessage("Your account was created successfully.");
                    NewAccountDialog.Hide();
                    NewAccountDialog.Clear();
                    LoginDialog.Show();
                    break;
#if DEBUG
                default:
                    throw new NotImplementedException();
#endif
            }
        }
        public static void ChangePassword(ChangePassword P)
        {
            ChangePasswordDialog.ConfirmButton.Enabled = true;

            switch (P.Result)
            {
                case 0:
                    SceneFunctions.ShowMessage("An error occured whilst Changing Password.");
                    ChangePasswordDialog.Hide();
                    ChangePasswordDialog.Clear();
                    LoginDialog.Show();
                    break;
                case 1:
                    SceneFunctions.ShowMessage("Password Changing is currently disabled.");
                    ChangePasswordDialog.Hide();
                    ChangePasswordDialog.Clear();
                    LoginDialog.Show();
                    break;
                case 2:
                    SceneFunctions.ShowMessage("Your AccountID is not acceptable.");
                    ChangePasswordDialog.AccountIDTextBox.SetFocus();
                    break;
                case 3:
                    SceneFunctions.ShowMessage("The current Password is not acceptable.");
                    ChangePasswordDialog.CurrentPasswordTextBox.SetFocus();
                    break;
                case 4:
                    SceneFunctions.ShowMessage("Your new Password is not acceptable.");
                    ChangePasswordDialog.NewPassword1TextBox.SetFocus();
                    break;
                case 5:
                    SceneFunctions.ShowMessage("The AccountID does not exist.");
                    ChangePasswordDialog.AccountIDTextBox.SetFocus();
                    break;
                case 6:
                    SceneFunctions.ShowMessage("Incorrect Password and AccountID combination.");
                    ChangePasswordDialog.CurrentPasswordTextBox.SetFocus();
                    ChangePasswordDialog.CurrentPasswordTextBox.Text = string.Empty;
                    break;
                case 7:
                    SceneFunctions.ShowMessage("Your password was changed successfully.");
                    ChangePasswordDialog.Hide();
                    ChangePasswordDialog.Clear();
                    LoginDialog.Show();
                    break;
#if DEBUG
                default:
                    throw new NotImplementedException();
#endif
            }
        }
        public static void ChangePasswordBanned(ChangePasswordBanned P)
        {
            ChangePasswordDialog.ConfirmButton.Enabled = true;
            SceneFunctions.ShowMessage(string.Format("This account is banned.\n Reason{0}\n ExpiaryDate:{1}", P.Reason,
                                             P.ExpiryDate));
            ChangePasswordDialog.Clear();
        }
        public static void Login(Login P)
        {
            LoginDialog.LoginButton.Enabled = true;
            switch (P.Result)
            {
                case 0:
                    SceneFunctions.ShowMessage("An error occured when trying to Log in.");
                    LoginDialog.Clear();
                    break;
                case 1:
                    SceneFunctions.ShowMessage("Logging in is currently disabled.");
                    LoginDialog.Clear();
                    break;
                case 2:
                    SceneFunctions.ShowMessage("Your AccountID is not acceptable.");
                    LoginDialog.AccountIDTextBox.SetFocus();
                    break;
                case 3:
                    SceneFunctions.ShowMessage("Your Password is not acceptable.");
                    LoginDialog.PasswordTextBox.SetFocus();
                    break;
                case 4:
                    SceneFunctions.ShowMessage("The AccountID does not exist.");
                    LoginDialog.AccountIDTextBox.SetFocus();
                    break;
                case 5:
                    SceneFunctions.ShowMessage("Incorrect Password and AccountID combination.");
                    LoginDialog.PasswordTextBox.Text = string.Empty;
                    LoginDialog.PasswordTextBox.SetFocus();
                    break;
#if DEBUG
                default:
                    throw new NotImplementedException();
#endif
            }
        }
        public static void LoginBanned(LoginBanned P)
        {
            LoginDialog.LoginButton.Enabled = true;

            SceneFunctions.ShowMessage(string.Format("This account is banned.\n Reason{0}\n ExpiaryDate:{1}", P.Reason,
                                             P.ExpiryDate));
        }
        public static void LoginSuccess(LoginSuccess P)
        {
            SceneFunctions.LoginSuccess();

            if (P.CharacterList != null)
                P.CharacterList.Sort(delegate(SelectCharacterInfo S1, SelectCharacterInfo S2) { return S2.LastAccess.CompareTo(S1.LastAccess); });

            SelectScene.CharacterList = P.CharacterList;
            SelectScene.UpdateSelectButtons();
        }

        public static void NewCharacter(NewCharacter P)
        {
            NewCharacterDialog.CharacterNameTextBox.Enabled = true;
            NewCharacterDialog.ConfirmButton.Enabled = true;

            switch (P.Result)
            {
                case 0:
                    SceneFunctions.ShowMessage("An error occured when trying create your character.");
                    break;
                case 1:
                    NewCharacterDialog.Hide();
                    SceneFunctions.ShowMessage("Creating new characters is currently disabled.");
                    break;
                case 2:
                    SceneFunctions.ShowMessage("Your Character Name is not acceptable.");
                    NewCharacterDialog.CharacterNameTextBox.SetFocus();
                    break;
                case 3:
                    SceneFunctions.ShowMessage("The gender you selected does not exist.\n Contact a GM for assistance.");
                    break;
                case 4:
                    SceneFunctions.ShowMessage("The class you selected does not exist.\n Contact a GM for assistance.");
                    break;
                case 5:
                    SceneFunctions.ShowMessage("You cannot make anymore then " + Globals.MaxCharacterCount + " Characters.");
                    NewCharacterDialog.Hide();
                    break;
                case 6:
                    SceneFunctions.ShowMessage("A Character with this name already exists.");
                    NewCharacterDialog.CharacterNameTextBox.SetFocus();
                    break;
#if DEBUG
                default:
                    throw new NotImplementedException();
#endif
            }
        }
        public static void NewCharacterSuccess(NewCharacterSuccess P)
        {
            NewCharacterDialog.CharacterNameTextBox.Enabled = true;
            NewCharacterDialog.ConfirmButton.Enabled = true;
            NewCharacterDialog.Hide();
            SceneFunctions.ShowMessage("Your character was created successfully.");

            SelectScene.CharacterList.Insert(0, P.CharInfo);
            SelectScene.SelectedIndex = 0;
            SelectScene.UpdateSelectButtons();
        }
        public static void DeleteCharacter(DeleteCharacter P)
        {
            switch (P.Result)
            {
                case 0:
                    SceneFunctions.ShowMessage("An error occured when trying delete your character.");
                    break;
                case 1:
                    SceneFunctions.ShowMessage("Deleting characters is currently disabled.");
                    break;
                case 2:
                    SceneFunctions.ShowMessage("The character you selected does not exist.\n Contact a GM for assistance.");
                    break;
                case 3:
                    for (int I = 0; I < SelectScene.CharacterList.Count; I++)
                        if (SelectScene.CharacterList[I].Index == SceneFunctions.DeleteIndex)
                        {
                            SelectScene.CharacterList.RemoveAt(I);
                            break;
                        }

                    SelectScene.UpdateSelectButtons();
                    SceneFunctions.ShowMessage("Your character was deleted successfully.");
                    break;
#if DEBUG
                default:
                    throw new NotImplementedException();
#endif
            }
        }

        public static void StartGame(StartGame P)
        {
            SelectScene.StartGameButton.Enabled = true;

            switch (P.Result)
            {
                case 0:
                    SceneFunctions.ShowMessage("An error occured when trying start the game.");
                    break;
                case 1:
                    SceneFunctions.ShowMessage("Starting the Game is currently disabled.");
                    break;
                case 2:
                    SceneFunctions.ShowMessage("The character you selected does not exist.\n Contact a GM for assistance.");
                    break;
                case 3:
                    SceneFunctions.ShowMessage("Server is not ready for Characters to play.");
                    break;
                case 4:
                    SelectScene.Scene.Enabled = false;
                    GameScene.Scene.Enabled = true;

                    MirScene.ActiveScene.Visible = false;
                    GameScene.Scene.Visible = true;
                    MirScene.ActiveScene = GameScene.Scene;
                    break;
            }
        }
        public static void StartGameDelay(StartGameDelay P)
        {
            SelectScene.StartGameButton.Enabled = true;

            SceneFunctions.ShowMessage(string.Format("You cannot start the game for another {0} seconds.", P.Seconds));
        }
        public static void StartGameBanned(StartGameBanned P)
        {
            SelectScene.StartGameButton.Enabled = true;
            SceneFunctions.ShowMessage(string.Format("This character is banned.\n Reason{0}\n ExpiaryDate:{1}", P.Reason,
                                             P.ExpiryDate));
        }
        public static void LogOutSuccess(LogOutSuccess P)
        {
            SceneFunctions.LogOutSuccess();

            if (P.CharacterList != null)
                P.CharacterList.Sort(delegate(SelectCharacterInfo S1, SelectCharacterInfo S2) { return S2.LastAccess.CompareTo(S1.LastAccess); });

            SelectScene.CharacterList = P.CharacterList;
            SelectScene.UpdateSelectButtons();
        }


        public static void MapInfomation(MapInfomation P)
        {
            if (P.Details == null) return;

            MapLayer.LoadMap(P.Details);            
        }
        public static void UserInformation(UserInformation P)
        {
            if (P.Details == null) return;

            MapObject.User.NewInfo(P.Details);
        }
        public static void UpdateUserStats(UpdateUserStats P)
        {
            MapObject.User.NewStats(P.Details);
        }
        public static void UserInventory(UserInventory P)
        {
            MapObject.User.NewInventory(P);
        }
        public static void PlayerLocation(PlayerLocation P)
        {
            if (MapObject.User.Location != P.Location)
            {
                MapObject.User.DoAction(MirAction.Standing);
                MapObject.User.Location = P.Location;
                MapObject.User.Direction = P.Direction;
            }
            Main.NextMoveTime = 0;
        }
        public static void WinExperience(WinExperience P)
        {
            ChatPanel.RecieveChat(new ChatInfo { Message = string.Format("Experience Gained {0}.", P.Amount), Type = MirChatType.Experience });
            MapObject.User.CurrentExperience += P.Amount;
        }

        public static void ObjectChat(ObjectChat P)
        {
            /*string ObjectName = null;

            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                if (MapLayer.ObjectList[I] == null || MirMapControl.ObjectList[I].ObjectID != P.ObjectID) continue;
                ObjectName = MapLayer.ObjectList[I].Name;
                break;
            }*/

            //Add Speach Bubble
            ChatPanel.RecieveChat(new ChatInfo { User = P.Name, Message = P.Message, Type = P.Type });
        }

        public static void NewPlayerObject(NewPlayerObject P)
        {
            PlayerObject O = new PlayerObject();
            O.NewInfo(P.Details);
            MapLayer.AddObject(O);
        }
        public static void NewMonsterObject(NewMonsterObject P)
        {
            MapLayer.AddObject(new MonsterObject(P.Details));
        }
        public static void NewItemObject(NewItemObject P)
        {
            MapLayer.AddObject(new ItemObject(P.Details));
        }
        public static void RemoveMapObject(RemoveMapObject P)
        {
            if (MapObject.User.ObjectID == P.ObjectID) return;

            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
                if (MapLayer.ObjectList[I].ObjectID == P.ObjectID)
                {
                    MapLayer.ObjectList[I].Remove();
                    break;
                }


            if (PlayerObject.TargetObject != null && PlayerObject.TargetObject.ObjectID == P.ObjectID)
                PlayerObject.TargetObject = null;

            if (PlayerObject.MouseObject != null && PlayerObject.MouseObject.ObjectID == P.ObjectID)
                PlayerObject.MouseObject = null;
        }
        public static void ObjectTurn(ObjectTurn P)
        {
            MapObject Temp;
            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                if ((Temp = MapLayer.ObjectList[I]) == MapObject.User) continue;

                if (Temp.ObjectID == P.ObjectID)
                {
                    Temp.QueueAction(MirAction.Turn, P.Direction);
                    return;
                }
            }
        }
        public static void ObjectWalk(ObjectWalk P)
        {
            MapObject Temp;
            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                if ((Temp = MapLayer.ObjectList[I]) == MapObject.User) continue;

                if (Temp.ObjectID == P.ObjectID)
                {
                    Temp.QueueAction(MirAction.Walking, P.Location);
                    Temp.ActualLocation = P.Location;
                    return;
                }
            }
        }

        public static void ObjectRun(ObjectRun P)
        {
            MapObject Temp;
            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                if ((Temp = MapLayer.ObjectList[I]) == MapObject.User) continue;

                if (Temp.ObjectID == P.ObjectID)
                {
                    Temp.QueueAction(MirAction.Running, P.Location);
                    Temp.ActualLocation = P.Location;
                    return;
                }
            }
        }

        public static void ObjectAttack(ObjectAttack P)
        {
            MapObject Temp;
            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                if ((Temp = MapLayer.ObjectList[I]) == MapObject.User) continue;

                if (Temp.ObjectID == P.ObjectID)
                {
                    switch (P.Type)
                    {
                        case 2:
                            Temp.QueueAction(MirAction.Attack3, P.Direction); //type
                            return;
                        case 1:
                            Temp.QueueAction(MirAction.Attack2, P.Direction); //type
                            return;
                        default:
                            Temp.QueueAction(MirAction.Attack1, P.Direction); //type
                            return;
                    }
                }
            }
        }
        public static void ObjectHarvest(ObjectHarvest P)
        {
            MapObject Temp;
            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                if ((Temp = MapLayer.ObjectList[I]) == MapObject.User) continue;

                if (Temp.ObjectID == P.ObjectID)
                {
                    Temp.QueueAction(MirAction.Harvest, P.Direction);
                    return;
                }
            }
        }
        public static void ObjectSkeleton(ObjectSkeleton P)
        {
            MapObject Temp;

            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                if ((Temp = MapLayer.ObjectList[I]) == MapObject.User) continue;

                if (Temp.ObjectID == P.ObjectID)
                {
                    Temp.QueueAction(MirAction.Skeleton, 0);
                    //Temp.Skeleton = true;
                    return;
                }
            }
        }
        public static void ObjectStruck(ObjectStruck P)
        {
            MapObject Temp;
            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                Temp = MapLayer.ObjectList[I];

                if (Temp.ObjectID == P.ObjectID)
                {
                    Temp.AttackerID = P.AttackerID;
                    Temp.Struck = true;
                    return;
                }
            }
        }

        public static void ObjectDied(ObjectDied P)
        {
            MapObject Temp;
            if (MapObject.User.ObjectID == P.ObjectID)
            {
                MapObject.User.DoAction(MirAction.Die);
                GameScene.OutputMessage("You died.");
                return;
            }

            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                if ((Temp = MapLayer.ObjectList[I]) == MapObject.User) continue;

                if (Temp.ObjectID == P.ObjectID)
                {
                    Temp.QueueAction(MirAction.Die, 0);
                    return;
                }
            }
        }
        public static void ObjectHealthChanged(ObjectHealthChanged P)
        {
            MapObject Temp;
            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                Temp = MapLayer.ObjectList[I];

                if (Temp.ObjectID == P.ObjectID)
                {
                    Temp.CurrentHP = P.HP;
                    Temp.MaxHP = P.MaxHP;
                    return;
                }
            }
        }

        public static void GainedItem(GainedItem P)
        {
            MapObject.User.CalculateWeight();

            MapObject.User.AddBagItem(P.Item);
            GameScene.OutputMessage(string.Format("You gained {0}.", P.Item.Info.ItemName));
        }
        public static void GainedGold(GainedGold P)
        {
            MapObject.User.Gold += P.Amount;
            SoundManager.PlaySound(SoundList.Gold, false);
            GameScene.OutputMessage(string.Format("You gained {0:###,###,###} Gold.", P.Amount));
        }

        public static void WeightChanged(WeightChanged P)
        {
            MapObject.User.CalculateWeight();
        }
    }
}
