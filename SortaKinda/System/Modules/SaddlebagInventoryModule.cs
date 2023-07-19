namespace SortaKinda.System.Modules;

// Saddlebag Not Supported for now.
// public class SaddlebagInventoryModule : InventoryModuleBase
// {
//     public override ModuleName ModuleName { get; protected set; } = ModuleName.SaddlebagInventory;
//     public override IModuleConfig ModuleConfig { get; set; } = new SaddlebagConfig();
//
//     private InventoryGrid inventory1 = null!;
//     private InventoryGrid inventory2 = null!;
//
//     private int currentInventoryCount;
//     
//     public override void DrawInventoryGrid()
//     {
//         var region = ImGui.GetContentRegionAvail();
//         
//         var firstPosition = new Vector2(0.0f, region.Y / 4.0f);
//         var secondPosition = new Vector2(region.X / 2.0f, region.Y / 4.0f);
//         
//         inventory1.Draw(firstPosition);
//         inventory2.Draw(secondPosition);
//     }
//     
//     protected override void LoadModule()
//     {
//         ModuleConfig.Configurations ??= new Dictionary<InventoryType, InventoryConfig>
//         {
//             { InventoryType.SaddleBag1, new InventoryConfig(InventoryType.SaddleBag1) },
//             { InventoryType.SaddleBag2, new InventoryConfig(InventoryType.SaddleBag2) },
//         };
//
//         inventory1 = new InventoryGrid(InventoryType.SaddleBag1, this);
//         inventory2 = new InventoryGrid(InventoryType.SaddleBag2, this);
//     }
//     
//     public override void SortAll()
//     {
//         SortController.SortInventory(InventoryType.SaddleBag1, inventory1, inventory2);
//     }
//     
//     public override void Update()
//     {
//         var count = InventoryController.GetInventoryItemCount(InventoryType.SaddleBag1, InventoryType.SaddleBag2);
//
//         if (currentInventoryCount != count)
//         {
//             SortController.SortInventory(InventoryType.SaddleBag1, inventory1, inventory2);
//             currentInventoryCount = count;
//         }
//     }
// }