using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace lookatme
{
    class ILCursor
    {
        private ILProcessor ilProcessor { get; set; }
        private Instruction cursor { get; set; }

        // 가상의 Head -1 를 가리키는 상태를 나타냄
        private bool emptyHead { get; set; }

        public Instruction current
        {
            get
            {
                return cursor;
            }
        }
        public ILCursor clone
        {
            get
            {
                return new ILCursor(ilProcessor, cursor);
            }
        }

        public ILCursor(ILProcessor ilProcessor)
        {
            this.ilProcessor = ilProcessor;
            this.cursor = ilProcessor.Body.Instructions.First();
        }
        public ILCursor(ILProcessor ilProcessor, bool emptyHead)
        {
            this.ilProcessor = ilProcessor;
            this.cursor = ilProcessor.Body.Instructions.First();
            this.emptyHead = true;
        }
        public ILCursor(ILProcessor ilProcessor, Instruction cursor)
        {
            this.ilProcessor = ilProcessor;
            this.cursor = cursor;
        }

        /// <summary>
        /// 커서 오른쪽에 끼워넣는다.
        /// 커서는 새로 넣어진 명령어를 가리킨다.
        /// </summary>
        /// <param name="inst">명렁어</param>
        public void Emit(Instruction inst)
        {
            if (emptyHead)
            {
                ilProcessor.InsertBefore(cursor, inst);
                cursor = inst;
                emptyHead = false;
            }
            else
            {
                ilProcessor.InsertAfter(cursor, inst);
                cursor = cursor.Next;
            }
        }
        public void Emit(params Instruction[] insts)
        {
            foreach (var inst in insts)
                Emit(inst);
        }

        /// <summary>
        /// 커서 왼쪽에 끼워넣는다.
        /// 커서는 변하지 않는다.
        /// </summary>
        /// <param name="inst">명렁어</param>
        public void EmitBefore(Instruction inst)
        {
            ilProcessor.InsertBefore(cursor, inst);
        }
        public void EmitBefore(params Instruction[] insts)
        {
            foreach (var inst in insts)
                EmitBefore(inst);
        }

        /// <summary>
        /// 현재 커서 명령을 새로운 명령으로 교체한다.
        /// 커서의 위치 자체는 바뀌지 않는다.
        /// </summary>
        /// <param name="inst">명령어</param>
        public void Replace(Instruction inst)
        {
            ilProcessor.Replace(cursor, inst);
            cursor = inst;
        }

        /// <summary>
        /// 현재 커서 명령을 삭제한다.
        /// 커서는 현재 커서의 Next로 변화한다.
        /// </summary>
        /// <param name="inst"></param>
        public void Remove()
        {
            var next = cursor.Next;
            ilProcessor.Remove(cursor);
            cursor = next;
        }

        #region SHORTCUTS
        public void LdThis(ILProcessor ilgen)
        {
            Emit(ilgen.Create(OpCodes.Ldarg_0));
        }
        public void LdStr(ILProcessor ilgen, string str)
        {
            Emit(ilgen.Create(OpCodes.Ldstr, str));
        }
        public void Call(ILProcessor ilgen, MethodReference method)
        {
            Emit(ilgen.Create(OpCodes.Call, method));
        }
        public void Callvirt(ILProcessor ilgen, MethodReference method)
        {
            Emit(ilgen.Create(OpCodes.Callvirt, method));
        }
        public void Dup(ILProcessor ilgen)
        {
            Emit(ilgen.Create(OpCodes.Dup));
        }
        #endregion // SHORTCUTS
    }
}