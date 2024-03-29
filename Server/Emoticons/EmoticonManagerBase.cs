﻿namespace Server.Emoticons
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    public class EmoticonManagerBase
    {
        #region Fields

        static EmoticonCollection emoticons;

        #endregion Fields

        #region Events

        public static event EventHandler LoadComplete;

        public static event EventHandler<LoadingUpdateEventArgs> LoadUpdate;

        #endregion Events

        #region Properties

        public static EmoticonCollection Emoticons {
            get { return emoticons; }
        }

        #endregion Properties

        #region Methods

        public static void CheckEmotions() {
            if (System.IO.File.Exists(IO.Paths.DataFolder + "emoticons.xml") == false) {
                //SaveEmotions();
            }
        }

        public static void Initialize(int maxEmotions) {
            emoticons = new EmoticonCollection(maxEmotions);
            CheckEmotions();
        }

        public static void LoadEmotions(object object1) {
            try {
                using (XmlReader reader = XmlReader.Create(IO.Paths.DataFolder + "emoticons.xml")) {
                    while (reader.Read()) {
                        if (reader.IsStartElement()) {
                            switch (reader.Name) {
                                case "Emoticon": {
                                        string idval = reader["id"];
                                        int id = 0;
                                        if (idval != null) {
                                            id = idval.ToInt();
                                        }
                                        emoticons[id] = new Emoticon();
                                        if (reader.Read()) {
                                            emoticons[id].Pic = reader.ReadElementString("Pic").ToInt();
                                            emoticons[id].Command = reader.ReadElementString("Command");
                                        }
                                        if (LoadUpdate != null)
                                            LoadUpdate(null, new LoadingUpdateEventArgs(id, emoticons.MaxEmoticons));
                                    }
                                    break;
                            }
                        }
                    }
                }
                if (LoadComplete != null)
                    LoadComplete(null, null);
            } catch (Exception ex) {
                Exceptions.ErrorLogger.WriteToErrorLog(ex);
            }
        }

        #endregion Methods
    }
}