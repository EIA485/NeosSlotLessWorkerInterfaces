using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using FrooxEngine.LogiX;

namespace SlotLessWorkerInterfaces
{
    public class SlotLessWorkerInterfaces : NeosMod
    {
        public override string Name => "SlotLessWorkerInterfaces";
        public override string Author => "eia485";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/EIA485/NeosSlotLessWorkerInterfaces/";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.eia485.SlotLessWorkerInterfaces");
            harmony.PatchAll();
        }

        [HarmonyPatch]
        class SlotLessWorkerInterfacesPatch
        {
            [HarmonyPatch(typeof(LogixHelper), "GetLogixInterfaceProxy")]
            static bool Prefix(ref LogixInterfaceProxy __result, Worker worker, bool create)
            {
                if (worker.IsDisposed) return true; // return true so the original method does the handeling incase that ever changes
                if (worker.FindNearestParent<Slot>() == null && create)
                {
                    var slot = worker.World.RootSlot.FindOrAdd("ProxyHolder");
                    __result = slot.GetComponent((LogixInterfaceProxy p) => p.Worker == worker);
                    if (__result == null)
                    {
                        __result = slot.AttachComponent<LogixInterfaceProxy>();
                        __result.Setup(worker);
                    }
                    return false;
                }
                return true;
            }
        }
    }
}