namespace Main
{
  class x86_64
  {
    private const int KVM_NR_INTERRUPTS = 256; // Arch interrupt line count
    public struct KvmSegment
    {
      public KvmSegment(ulong _base, uint _limit, ushort _selector, byte _type_, byte _present, byte _dpl, byte _db, byte _s, byte _l, byte _g, byte _avl, byte _unusable, byte _padding)
      {
        @base = _base;
        limit = _limit;
        selector = _selector;
        type_ = _type_;
        present = _present;
        dpl = _dpl;
        db = _db;
        s = _s;
        l = _l;
        g = _g;
        avl = _avl;
        unusable = _unusable;
        padding = _padding;
      }
      ulong @base;
      uint limit;
      ushort selector;
      byte type_;
      byte present;
      byte dpl;
      byte db;
      byte s;
      byte l;
      byte g;
      byte avl;
      byte unusable;
      byte padding;

    }

    public struct KvmDtable
    {
      // Array needs to be of lenght 3
      public KvmDtable(ulong _base, ushort _limit, ushort[] _padding)
      {
        @base = _base;
        limit = _limit;
        padding = new ushort[3];
        for (int i = 0; i < 3; i++)
        {
          padding.Append(_padding[i]);
        }

      }
      ulong @base;
      ushort limit;
      ushort[] padding;
    }

    public struct KvmSregs
    {
      // array needs to be of lenght 4
      public KvmSregs(KvmSegment _cs, KvmSegment _ds, KvmSegment _es, KvmSegment _fs, KvmSegment _gs, KvmSegment _ss, KvmSegment _tr, KvmSegment _ldt, KvmDtable _gdt, KvmDtable _idt, ulong _cr0, ulong _cr2, ulong _cr3, ulong _cr4, ulong _cr8, ulong _efer, ulong _apic_base, ulong[] _interrupt_bitmap)
      {
        cs = _cs;
        ds = _ds;
        es = _es;
        fs = _fs;
        gs = _gs;
        ss = _ss;
        tr = _tr;
        ldt = _ldt;
        gdt = _gdt;
        idt = _idt;
        cr0 = _cr0;
        cr2 = _cr2;
        cr3 = _cr3;
        cr4 = _cr4;
        cr8 = _cr8;
        efer = _efer;
        apic_base = _apic_base;
        interrupt_bitmap = new ulong[4];
        for (int i = 0; i < 4; i++)
        {
          interrupt_bitmap.Append(_interrupt_bitmap[i]);
        }
      }
      KvmSegment cs, ds, es, fs, gs, ss, tr, ldt;
      KvmDtable gdt, idt;
      ulong cr0, cr2, cr3, cr4, cr8;
      ulong efer;
      ulong apic_base;
      ulong[] interrupt_bitmap;
    }
  }
}