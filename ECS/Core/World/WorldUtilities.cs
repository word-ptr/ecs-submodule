namespace ME.ECS {

    #if ECS_COMPILE_IL2CPP_OPTIONS
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.NullChecks, false),
     Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false),
     Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    #endif
    public static class WorldUtilities {

        private static class TypesCache<T> {

            internal static int typeId = 0;

        }

        public static void SetWorld(World world) {

            Worlds.currentWorld = world;
            
        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static TState CreateState<TState>() where TState : State, new() {

            var state = PoolClass<TState>.Spawn();
            state.entityId = default;
            state.tick = default;
            state.randomState = default;
            return state;

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void ReleaseState<TState>(ref State state) where TState : State, new() {

            state.entityId = default;
            state.tick = default;
            state.randomState = default;
            PoolClass<TState>.Recycle((TState)state);
            state = null;

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void ReleaseState<TState>(ref TState state) where TState : State, new() {

            state.entityId = default;
            state.tick = default;
            state.randomState = default;
            PoolStates<TState>.Recycle(ref state);

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Release(ref Storage storage) {
            
            PoolClass<Storage>.Recycle(ref storage);
            
        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Release(ref FiltersStorage storage) {

            if (storage == null) return;
            PoolClass<FiltersStorage>.Recycle(ref storage);
            
        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Release(ref StructComponentsContainer storage) {
            
            //PoolClass<StructComponentsContainer>.Recycle(ref storage);
            storage.OnRecycle();
            
        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void Release(ref Components components) {
            
            PoolClass<Components>.Recycle(ref components);
            
        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void CreateWorld<TState>(ref World worldRef, float tickTime, int forcedWorldId = 0, uint seed = 1u) where TState : State, new() {

            if (seed <= 0u) seed = 1u;
            
            if (worldRef != null) WorldUtilities.ReleaseWorld<TState>(ref worldRef);
            worldRef = PoolClass<World>.Spawn();
            worldRef.SetId(forcedWorldId);
            var worldInt = worldRef;
            worldInt.SetSeed(seed);
            worldInt.SetTickTime(tickTime);
            Worlds.Register(worldRef);

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void ReleaseWorld<TState>(ref World world) where TState : State, new() {

            if (world == null || world.isActive == false) {

                world = null;
                return;
                
            }
            
            Worlds.DeInitializeBegin();
            var w = world;
            Worlds.currentWorld = w;
            world.RecycleResetState<TState>();
            PoolClass<World>.Recycle(ref w);
            world.RecycleStates<TState>();
            Worlds.UnRegister(world);
            Worlds.DeInitializeEnd();

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int GetKey<T>() {

            if (TypesCache<T>.typeId == 0) TypesCache<T>.typeId = typeof(T).GetHashCode();
            return TypesCache<T>.typeId;

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int GetKey(in System.Type type) {

            return type.GetHashCode();

        }

        public static void ResetTypeIds() {

            AllComponentTypesCounter.counter = -1;
            ComponentTypesRegistry.allTypeId.Clear();
            if (ComponentTypesRegistry.reset != null) ComponentTypesRegistry.reset.Invoke();
            ComponentTypesRegistry.reset = null;

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool IsAllComponentInHash<TComponent>() {

            return AllComponentTypes<TComponent>.isInHash;

        }
        
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void SetAllComponentInHash<TComponent>(bool state) {
            
            AllComponentTypes<TComponent>.isInHash = state;
            
        }
        
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int GetAllComponentTypeId<TComponent>() {

            if (AllComponentTypes<TComponent>.typeId < 0) {
                
                AllComponentTypes<TComponent>.typeId = ++AllComponentTypesCounter.counter;
                ComponentTypesRegistry.allTypeId.Add(typeof(TComponent), AllComponentTypes<TComponent>.typeId);
                
                ComponentTypesRegistry.reset += () => {

                    AllComponentTypes<TComponent>.typeId = -1;
                    AllComponentTypes<TComponent>.isTag = false;

                };

            }
            
            return AllComponentTypes<TComponent>.typeId;

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int GetComponentTypeId<TComponent>() {

            return ComponentTypes<TComponent>.typeId;

        }
        
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void SetComponentTypeId<TComponent>() {

            if (ComponentTypes<TComponent>.typeId < 0) {
                
                ComponentTypes<TComponent>.typeId = ++ComponentTypesCounter.counter;
                ComponentTypesRegistry.typeId.Add(typeof(TComponent), ComponentTypes<TComponent>.typeId);
                
            }
            
        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool IsComponentAsTag<TComponent>() where TComponent : struct, IStructComponent {

            return AllComponentTypes<TComponent>.isTag;

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void SetComponentAsTag<TComponent>() {

            AllComponentTypes<TComponent>.isTag = true;

        }

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static void InitComponentTypeId<TComponent>(bool isTag = false) {

            if (isTag == true) WorldUtilities.SetComponentAsTag<TComponent>();
            WorldUtilities.GetAllComponentTypeId<TComponent>();

        }

    }

}