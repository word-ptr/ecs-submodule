﻿
namespace ME.ECS.Collections {

    public struct IntrusiveHashSetBucket : IStructComponent {

        public IntrusiveList list;

    }

    public interface IIntrusiveHashSet {

        int Count { get; }
        
        void Add(in Entity entityData);
        bool Remove(in Entity entityData);
        int RemoveAll(in Entity entityData);
        void Clear(bool destroyData = false);
        bool Contains(in Entity entityData);

        BufferArray<Entity> ToArray();
        IntrusiveHashSet.IntrusiveHashSetEnumerator GetEnumerator();

    }
    
    #if ECS_COMPILE_IL2CPP_OPTIONS
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    #endif
    public struct IntrusiveHashSet : IIntrusiveHashSet {

        #if ECS_COMPILE_IL2CPP_OPTIONS
        [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
        [Unity.IL2CPP.CompilerServices.Il2CppSetOptionAttribute(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
        #endif
        public struct IntrusiveHashSetEnumerator : System.Collections.Generic.IEnumerator<Entity> {

            private IntrusiveHashSet hashSet;
            private int bucketIndex;
            private IntrusiveList.IntrusiveListEnumerator listEnumerator;
            public Entity Current => this.listEnumerator.Current;

            [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public IntrusiveHashSetEnumerator(IntrusiveHashSet hashSet) {

                this.hashSet = hashSet;
                this.bucketIndex = 0;
                this.listEnumerator = default;
                
            }

            [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() {

                while (this.bucketIndex <= this.hashSet.buckets.Length) {

                    if (this.listEnumerator.MoveNext() == true) {

                        return true;

                    }

                    var bucket = this.hashSet.buckets[this.bucketIndex];
                    if (bucket.IsAlive() == true) {

                        var node = bucket.GetData<IntrusiveHashSetBucket>();
                        this.listEnumerator = node.list.GetEnumerator();
                    
                    }
                    ++this.bucketIndex;

                }

                return false;

            }

            [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void Reset() {
                
                this.bucketIndex = 0;
                this.listEnumerator = default;

            }

            object System.Collections.IEnumerator.Current {
                get {
                    throw new AllocationException();
                }
            }

            public void Dispose() {

            }

        }

        private StackArray10<Entity> buckets;
        private int count;

        public int Count => this.count;

        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public IntrusiveHashSetEnumerator GetEnumerator() {

            return new IntrusiveHashSetEnumerator(this);

        }

        /// <summary>
        /// Put entity data into array.
        /// </summary>
        /// <returns>Buffer array from pool</returns>
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public BufferArray<Entity> ToArray() {

            var arr = PoolArray<Entity>.Spawn(this.count);
            var i = 0;
            foreach (var entity in this) {
                
                arr.arr[i++] = entity;
                
            }

            return arr;

        }

        /// <summary>
        /// Find an element.
        /// </summary>
        /// <param name="entityData"></param>
        /// <returns>Returns TRUE if data was found</returns>
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Contains(in Entity entityData) {
            
            var bucket = entityData.GetHashCode() % this.buckets.Length;
            var bucketEntity = this.buckets[bucket];
            if (bucketEntity.IsAlive() == false) return false;
            
            ref var bucketList = ref bucketEntity.GetData<IntrusiveHashSetBucket>();
            return bucketList.list.Contains(entityData);

        }
        
        /// <summary>
        /// Clear the list.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Clear(bool destroyData = false) {

            for (int i = 0; i < this.buckets.Length; ++i) {

                var bucket = this.buckets[i];
                if (bucket.IsAlive() == true) {
                    
                    ref var data = ref bucket.GetData<IntrusiveHashSetBucket>();
                    data.list.Clear();
                    
                }

            }
            
            this.count = 0;

        }

        /// <summary>
        /// Remove data from list.
        /// </summary>
        /// <param name="entityData"></param>
        /// <returns>Returns TRUE if data was found</returns>
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool Remove(in Entity entityData) {

            var bucket = entityData.GetHashCode() % this.buckets.Length;
            var bucketEntity = this.buckets[bucket];
            if (bucketEntity.IsAlive() == false) return false;
            
            ref var bucketList = ref bucketEntity.GetData<IntrusiveHashSetBucket>(false);
            if (bucketList.list.Remove(entityData) == true) {

                --this.count;
                return true;

            }
            
            return false;
            
        }

        /// <summary>
        /// Remove all nodes data from list.
        /// </summary>
        /// <param name="entityData"></param>
        /// <returns>Returns TRUE if data was found</returns>
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public int RemoveAll(in Entity entityData) {

            var bucket = entityData.GetHashCode() % this.buckets.Length;
            var bucketEntity = this.buckets[bucket];
            if (bucketEntity.IsAlive() == false) return 0;
            
            ref var bucketList = ref bucketEntity.GetData<IntrusiveHashSetBucket>(false);
            var count = bucketList.list.RemoveAll(in entityData);
            this.count -= count;
            return count;

        }

        /// <summary>
        /// Add new data at the end of the list.
        /// </summary>
        /// <param name="entityData"></param>
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Add(in Entity entityData) {

            IntrusiveHashSet.Initialize(ref this);

            var bucket = entityData.GetHashCode() % this.buckets.Length;
            var bucketEntity = this.buckets[bucket];
            if (bucketEntity.IsAlive() == false) bucketEntity = this.buckets[bucket] = new Entity("IntrusiveHashSetBucket");
            ref var bucketList = ref bucketEntity.GetData<IntrusiveHashSetBucket>();
            bucketList.list.Add(entityData);
            ++this.count;

        }

        #region Helpers
        [System.Runtime.CompilerServices.MethodImplAttribute(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static void Initialize(ref IntrusiveHashSet hashSet) {

            if (hashSet.buckets.Length == 0) hashSet.buckets = new StackArray10<Entity>(10);

        }
        #endregion

    }

}