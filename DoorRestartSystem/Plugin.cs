﻿using System;
using EXILED;
using EXILED.Extensions;
using MEC;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoorRestartSystem
{

    public class Plugin : EXILED.Plugin
    {
        public Random Gen = new Random();

        public EventHandlers EventHandlers;
        public MTFRespawn Respawn;
        public static bool TimerOn = true;

        public float InitialDelay;
        public float DurationMin;
        public float DurationMax;
        public static int DelayMax;
        public static int DelayMin;


        public override void OnEnable()
        {
            try
            {
                Log.Info("loaded.");
                ReloadConfig();
                Log.Info("Configs loaded.");
                EventHandlers = new EventHandlers(this);

                Events.RoundStartEvent += EventHandlers.OnRoundStart;
                Events.RoundEndEvent += EventHandlers.OnRoundEnd;
                Events.WaitingForPlayersEvent += EventHandlers.OnWaitingForPlayers;
                Events.TriggerTeslaEvent += EventHandlers.OnTriggerTesla;
            }
            catch (Exception e)
            {
                Log.Error($"OnEnable Error: {e}");
            }
        }

        public void ReloadConfig()
        {
            Log.Info($"Config Path: {Config.Path}");
            InitialDelay = Config.GetFloat("drs_initial_delay", 120f);
            DurationMin = Config.GetFloat("drs_dur_min", 5f);
            DurationMax = Config.GetFloat("drs_dur_max", 10);
            DelayMin = Config.GetInt("drs_delay_min", 180);
            DelayMax = Config.GetInt("drs_delay_max", 300);
           
        }
        public override void OnDisable()
        {
            foreach (CoroutineHandle handle in EventHandlers.Coroutines)
                Timing.KillCoroutines(handle);
            Events.RoundStartEvent -= EventHandlers.OnRoundStart;
            Events.RoundEndEvent -= EventHandlers.OnRoundEnd;
            Events.WaitingForPlayersEvent -= EventHandlers.OnWaitingForPlayers;
            Events.TriggerTeslaEvent -= EventHandlers.OnTriggerTesla;
            EventHandlers = null;
        }

        public override void OnReload()
        {

        }

        public override string getName
        {
            get;
        } = "DoorRestartSystem";

        Random rand = new Random();

       

        public static int getRandom()
        {
            Random rand = new Random();
            return rand.Next(DelayMin, DelayMax);
        }

        public static int randomValue = getRandom();






        public IEnumerator<float> RunBlackoutTimer()
        {
            if (Respawn == null)
                Respawn = PlayerManager.localPlayer.GetComponent<MTFRespawn>();
            yield
            return Timing.WaitForSeconds(InitialDelay);

            for (; ; )
            {
                Respawn.RpcPlayCustomAnnouncement("WARNING . DOOR SOFTWARE REPAIR IN t minus 20 seconds .", false, true);


                TimerOn = true;
                yield
                return Timing.WaitForSeconds(23f);
                float blackoutDur = DurationMax;
                



                foreach (Door door in UnityEngine.Object.FindObjectsOfType<Door>())
                {


                    door.SetStateWithSound(false);
                    door.Networklocked = true;
                }

                yield
                return Timing.WaitForSeconds(blackoutDur);
                foreach (Door door in UnityEngine.Object.FindObjectsOfType<Door>())
                {
                    door.Networklocked = false;
                }
                Respawn.RpcPlayCustomAnnouncement("DOOR SOFTWARE REPAIR COMPLETE", false, true);
                yield
                return Timing.WaitForSeconds(randomValue);
                TimerOn = false;

            }
        }
    }

}