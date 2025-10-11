using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using SCPTroubleInTerroristTown;
using UnityEngine.Pool;

[HarmonyPatch(typeof(FpcNoclipToggleMessage), nameof(FpcNoclipToggleMessage.ProcessMessage))]
public class NoClipTogglePatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = instructions.ToList();

        Label ret = generator.DefineLabel();

        newInstructions[newInstructions.Count - 1].labels.Add(ret);

        int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + 1;

        newInstructions.InsertRange(index, new CodeInstruction[]
        {
            new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
            new (OpCodes.Call, AccessTools.Method(typeof(PatchEvents), nameof(PatchEvents.OnPlayerTogglingNoClip))),
            new (OpCodes.Brfalse, ret),
        });

        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
    }
}