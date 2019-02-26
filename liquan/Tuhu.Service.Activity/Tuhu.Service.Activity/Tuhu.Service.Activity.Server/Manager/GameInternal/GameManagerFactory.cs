using System.Collections.Generic;
using System.Linq;
using Tuhu.Service.Activity.Models.Requests;

namespace Tuhu.Service.Activity.Server.Manager.GameInternal
{
    /// <summary>
    ///     游戏业务类工厂
    /// </summary>
    internal static class GameManagerFactory
    {
        private static readonly IList<DefaultGameManager> _gameList = new List<DefaultGameManager>();

        static GameManagerFactory()
        {
            _gameList.Add(new GameMaPaiManager());
            _gameList.Add(new GameBumblebeeManager());
        }


        /// <summary>
        ///     返回全部游戏
        /// </summary>
        /// <returns></returns>
        public static IList<DefaultGameManager> GetAllGameManagers()
        {
            return _gameList;
        }

        /// <summary>
        ///     返回对应业务
        /// </summary>
        /// <param name="gameVersion"></param>
        /// <returns></returns>
        public static DefaultGameManager GetGameManager(int gameVersion)
        {
            return _gameList.FirstOrDefault(p => p.GameVersion == gameVersion);
        }

        /// <summary>
        ///     返回对应业务
        /// </summary>
        /// <param name="gameVersion"></param>
        /// <returns></returns>
        public static DefaultGameManager GetGameManager(GameVersionEnum gameVersion)
        {
            return _gameList.FirstOrDefault(p => p.GameVersion == (int) gameVersion);
        }
    }
}
