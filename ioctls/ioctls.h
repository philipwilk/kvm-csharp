// I know i could import <sys/kvm.h> but that would be too easy
// KVM ioctl id
const int KVM_ID = 0xAE;

// sequence numbers
const int KVM_GET_API_VERSION_seq = 0x00;
const int KVM_CREATE_VM_seq = 0x01;
const int KVM_CHECK_EXTENSION_seq = 0x03;
const int KVM_GET_VCPU_MMAP_SIZE_seq = 0x04;
const int KVM_GET_SUPPORTED_CPUID_seq = 0x05;
const int KVM_CREATE_VCPU_seq = 0x41;
const int KVM_SET_USER_MEMORY_REGION_seq = 0x46;
const int KVM_SET_TSS_ADDR_seq = 0x47;
const int KVM_SET_IDENTITY_MAP_ADDR_seq = 0x48;
const int KVM_CREATE_IRQCHIP_seq = 0x60;
const int KVM_CREATE_PIT2_seq = 0x77;
const int KVM_RUN_seq = 0x80;
const int KVM_SET_REGS_seq = 0x82;
const int KVM_GET_SREGS_seq = 0x83;
const int KVM_SET_SREGS_seq = 0x84;
const int KVM_SET_CPUID2_seq = 0x90;

// EXIT codes
#define KVM_EXIT_IO 2
#define KVM_EXIT_SHUTDOWN 8
#define KVM_EXIT_IO_IN 0
#define KVM_EXIT_IO_OUT 1

// KVM_FEATURNS
#define KVM_CPUID_SIGNATURE 0x40000000
#define KVM_CPUID_FEATURES 0x40000001

// Structs
struct kvm_userspace_memory_region
{
  unsigned int slot;
  unsigned int flags;
  unsigned long long guest_phys_addr;
  unsigned long long memory_size;    /* bytes */
  unsigned long long userspace_addr; /* start of the userspace allocated memory */
};

struct kvm_segment
{
  unsigned long base;
  unsigned int limit;
  unsigned short selector;
  unsigned char type;
  unsigned char present, dpl, db, s, l, g, avl;
  unsigned char unusable;
  unsigned char padding;
};

struct kvm_dtable
{
  unsigned long base;
  unsigned short limit;
  unsigned short padding[3];
};

struct kvm_sregs
{
  struct kvm_segment cs, ds, es, fs, gs, ss;
  struct kvm_segment tr, ldt;
  struct kvm_dtable gdt, idt;
  unsigned long cr0, cr2, cr3, cr4, cr8;
  unsigned long efer;
  unsigned long apic_base;
  unsigned long interrupt_bitmap[4];
};

struct kvm_regs
{
  unsigned long rax;
  unsigned long rbx;
  unsigned long rcx;
  unsigned long rdx;
  unsigned long rsi;
  unsigned long rdi;
  unsigned long rsp;
  unsigned long rbp;
  unsigned long r8;
  unsigned long r9;
  unsigned long r10;
  unsigned long r11;
  unsigned long r12;
  unsigned long r13;
  unsigned long r14;
  unsigned long r15;
  unsigned long rip;
  unsigned long rflags;
};

struct kvm_pit_config
{
  unsigned int flags;
  unsigned int pad[15];
};

struct kvm_cpuid_entry2
{
  __u32 function;
  __u32 index;
  __u32 flags;
  __u32 eax;
  __u32 ebx;
  __u32 ecx;
  __u32 edx;
  __u32 padding[3];
};

/* for KVM_SET_CPUID2 */
struct kvm_cpuid2
{
  __u32 nent;
  __u32 padding;
  struct kvm_cpuid_entry2 entries[];
};

struct kvm_cpuid
{
  unsigned int nent;
  unsigned int padding;
  struct kvm_cpuid_entry2 entries[100];
};

struct kvm_debug_exit_arch
{
  __u32 exception;
  __u32 pad;
  __u64 pc;
  __u64 dr6;
  __u64 dr7;
};

struct kvm_hyperv_exit
{
#define KVM_EXIT_HYPERV_SYNIC 1
#define KVM_EXIT_HYPERV_HCALL 2
#define KVM_EXIT_HYPERV_SYNDBG 3
  __u32 type;
  __u32 pad1;
  union
  {
    struct
    {
      __u32 msr;
      __u32 pad2;
      __u64 control;
      __u64 evt_page;
      __u64 msg_page;
    } synic;
    struct
    {
      __u64 input;
      __u64 result;
      __u64 params[2];
    } hcall;
    struct
    {
      __u32 msr;
      __u32 pad2;
      __u64 control;
      __u64 status;
      __u64 send_page;
      __u64 recv_page;
      __u64 pending_page;
    } syndbg;
  } u;
};

struct kvm_xen_exit
{
#define KVM_EXIT_XEN_HCALL 1
  __u32 type;
  union
  {
    struct
    {
      __u32 longmode;
      __u32 cpl;
      __u64 input;
      __u64 result;
      __u64 params[6];
    } hcall;
  } u;
};

/* for KVM_GET/SET_VCPU_EVENTS */
struct kvm_vcpu_events
{
  struct
  {
    __u8 injected;
    __u8 nr;
    __u8 has_error_code;
    __u8 pending;
    __u32 error_code;
  } exception;
  struct
  {
    __u8 injected;
    __u8 nr;
    __u8 soft;
    __u8 shadow;
  } interrupt;
  struct
  {
    __u8 injected;
    __u8 pending;
    __u8 masked;
    __u8 pad;
  } nmi;
  __u32 sipi_vector;
  __u32 flags;
  struct
  {
    __u8 smm;
    __u8 pending;
    __u8 smm_inside_nmi;
    __u8 latched_init;
  } smi;
  struct
  {
    __u8 pending;
  } triple_fault;
  __u8 reserved[26];
  __u8 exception_has_payload;
  __u64 exception_payload;
};

/* kvm_sync_regs struct included by kvm_run struct */
struct kvm_sync_regs
{
  /* Members of this structure are potentially malicious.
   * Care must be taken by code reading, esp. interpreting,
   * data fields from them inside KVM to prevent TOCTOU and
   * double-fetch types of vulnerabilities.
   */
  struct kvm_regs regs;
  struct kvm_sregs sregs;
  struct kvm_vcpu_events events;
};

/* for KVM_RUN, returned by mmap(vcpu_fd, offset=0) */
struct kvm_run
{
  /* in */
  __u8 request_interrupt_window;
  __u8 immediate_exit;
  __u8 padding1[6];

  /* out */
  __u32 exit_reason;
  __u8 ready_for_interrupt_injection;
  __u8 if_flag;
  __u16 flags;

  /* in (pre_kvm_run), out (post_kvm_run) */
  __u64 cr8;
  __u64 apic_base;

#ifdef __KVM_S390
  /* the processor status word for s390 */
  __u64 psw_mask; /* psw upper half */
  __u64 psw_addr; /* psw lower half */
#endif
  union
  {
    /* KVM_EXIT_UNKNOWN */
    struct
    {
      __u64 hardware_exit_reason;
    } hw;
    /* KVM_EXIT_FAIL_ENTRY */
    struct
    {
      __u64 hardware_entry_failure_reason;
      __u32 cpu;
    } fail_entry;
    /* KVM_EXIT_EXCEPTION */
    struct
    {
      __u32 exception;
      __u32 error_code;
    } ex;
    /* KVM_EXIT_IO */
    struct
    {
#define KVM_EXIT_IO_IN 0
#define KVM_EXIT_IO_OUT 1
      __u8 direction;
      __u8 size; /* bytes */
      __u16 port;
      __u32 count;
      __u64 data_offset; /* relative to kvm_run start */
    } io;
    /* KVM_EXIT_DEBUG */
    struct
    {
      struct kvm_debug_exit_arch arch;
    } debug;
    /* KVM_EXIT_MMIO */
    struct
    {
      __u64 phys_addr;
      __u8 data[8];
      __u32 len;
      __u8 is_write;
    } mmio;
    /* KVM_EXIT_HYPERCALL */
    struct
    {
      __u64 nr;
      __u64 args[6];
      __u64 ret;
      __u32 longmode;
      __u32 pad;
    } hypercall;
    /* KVM_EXIT_TPR_ACCESS */
    struct
    {
      __u64 rip;
      __u32 is_write;
      __u32 pad;
    } tpr_access;
    /* KVM_EXIT_S390_SIEIC */
    struct
    {
      __u8 icptcode;
      __u16 ipa;
      __u32 ipb;
    } s390_sieic;
    /* KVM_EXIT_S390_RESET */
#define KVM_S390_RESET_POR 1
#define KVM_S390_RESET_CLEAR 2
#define KVM_S390_RESET_SUBSYSTEM 4
#define KVM_S390_RESET_CPU_INIT 8
#define KVM_S390_RESET_IPL 16
    __u64 s390_reset_flags;
    /* KVM_EXIT_S390_UCONTROL */
    struct
    {
      __u64 trans_exc_code;
      __u32 pgm_code;
    } s390_ucontrol;
    /* KVM_EXIT_DCR (deprecated) */
    struct
    {
      __u32 dcrn;
      __u32 data;
      __u8 is_write;
    } dcr;
    /* KVM_EXIT_INTERNAL_ERROR */
    struct
    {
      __u32 suberror;
      /* Available with KVM_CAP_INTERNAL_ERROR_DATA: */
      __u32 ndata;
      __u64 data[16];
    } internal;
    /*
     * KVM_INTERNAL_ERROR_EMULATION
     *
     * "struct emulation_failure" is an overlay of "struct internal"
     * that is used for the KVM_INTERNAL_ERROR_EMULATION sub-type of
     * KVM_EXIT_INTERNAL_ERROR.  Note, unlike other internal error
     * sub-types, this struct is ABI!  It also needs to be backwards
     * compatible with "struct internal".  Take special care that
     * "ndata" is correct, that new fields are enumerated in "flags",
     * and that each flag enumerates fields that are 64-bit aligned
     * and sized (so that ndata+internal.data[] is valid/accurate).
     *
     * Space beyond the defined fields may be used to store arbitrary
     * debug information relating to the emulation failure. It is
     * accounted for in "ndata" but the format is unspecified and is
     * not represented in "flags". Any such information is *not* ABI!
     */
    struct
    {
      __u32 suberror;
      __u32 ndata;
      __u64 flags;
      union
      {
        struct
        {
          __u8 insn_size;
          __u8 insn_bytes[15];
        };
      };
      /* Arbitrary debug data may follow. */
    } emulation_failure;
    /* KVM_EXIT_OSI */
    struct
    {
      __u64 gprs[32];
    } osi;
    /* KVM_EXIT_PAPR_HCALL */
    struct
    {
      __u64 nr;
      __u64 ret;
      __u64 args[9];
    } papr_hcall;
    /* KVM_EXIT_S390_TSCH */
    struct
    {
      __u16 subchannel_id;
      __u16 subchannel_nr;
      __u32 io_int_parm;
      __u32 io_int_word;
      __u32 ipb;
      __u8 dequeued;
    } s390_tsch;
    /* KVM_EXIT_EPR */
    struct
    {
      __u32 epr;
    } epr;
    /* KVM_EXIT_SYSTEM_EVENT */
    struct
    {
#define KVM_SYSTEM_EVENT_SHUTDOWN 1
#define KVM_SYSTEM_EVENT_RESET 2
#define KVM_SYSTEM_EVENT_CRASH 3
#define KVM_SYSTEM_EVENT_WAKEUP 4
#define KVM_SYSTEM_EVENT_SUSPEND 5
#define KVM_SYSTEM_EVENT_SEV_TERM 6
      __u32 type;
      __u32 ndata;
      union
      {
#ifndef __KERNEL__
        __u64 flags;
#endif
        __u64 data[16];
      };
    } system_event;
    /* KVM_EXIT_S390_STSI */
    struct
    {
      __u64 addr;
      __u8 ar;
      __u8 reserved;
      __u8 fc;
      __u8 sel1;
      __u16 sel2;
    } s390_stsi;
    /* KVM_EXIT_IOAPIC_EOI */
    struct
    {
      __u8 vector;
    } eoi;
    /* KVM_EXIT_HYPERV */
    struct kvm_hyperv_exit hyperv;
    /* KVM_EXIT_ARM_NISV */
    struct
    {
      __u64 esr_iss;
      __u64 fault_ipa;
    } arm_nisv;
    /* KVM_EXIT_X86_RDMSR / KVM_EXIT_X86_WRMSR */
    struct
    {
      __u8 error; /* user -> kernel */
      __u8 pad[7];
#define KVM_MSR_EXIT_REASON_INVAL (1 << 0)
#define KVM_MSR_EXIT_REASON_UNKNOWN (1 << 1)
#define KVM_MSR_EXIT_REASON_FILTER (1 << 2)
      __u32 reason; /* kernel -> user */
      __u32 index;  /* kernel -> user */
      __u64 data;   /* kernel <-> user */
    } msr;
    /* KVM_EXIT_XEN */
    struct kvm_xen_exit xen;
    /* KVM_EXIT_RISCV_SBI */
    struct
    {
      unsigned long extension_id;
      unsigned long function_id;
      unsigned long args[6];
      unsigned long ret[2];
    } riscv_sbi;
    /* KVM_EXIT_RISCV_CSR */
    struct
    {
      unsigned long csr_num;
      unsigned long new_value;
      unsigned long write_mask;
      unsigned long ret_value;
    } riscv_csr;
    /* KVM_EXIT_NOTIFY */
    struct
    {
#define KVM_NOTIFY_CONTEXT_INVALID (1 << 0)
      __u32 flags;
    } notify;
    /* Fix the size of the union. */
    char padding[256];
  };

/* 2048 is the size of the char array used to bound/pad the size
 * of the union that holds sync regs.
 */
#define SYNC_REGS_SIZE_BYTES 2048
  /*
   * shared registers between kvm and userspace.
   * kvm_valid_regs specifies the register classes set by the host
   * kvm_dirty_regs specified the register classes dirtied by userspace
   * struct kvm_sync_regs is architecture specific, as well as the
   * bits for kvm_valid_regs and kvm_dirty_regs
   */
  __u64 kvm_valid_regs;
  __u64 kvm_dirty_regs;
  union
  {
    struct kvm_sync_regs regs;
    char padding[SYNC_REGS_SIZE_BYTES];
  } s;
};

int KVM_CREATE_VM(int fd);
int KVM_GET_API_VERSION(int fd);
int KVM_SET_USER_MEMORY_REGION(int fd, struct kvm_userspace_memory_region);
unsigned int KVM_CHECK_EXTENSION(int fd, int extension_identifier);
int KVM_CREATE_VCPU(int fd, unsigned int vcpu_id);
int KVM_GET_and_SET_SREGS(int vcpu, short arm64);
int KVM_SET_REGS(int vcpu);
int KVM_SET_TSS_ADDR(int vm_fd);
int KVM_SET_IDENTITY_MAP_ADDR(int vm_fd);
int KVM_CREATE_IRQCHIP(int vm_fd);
int KVM_CREATE_PIT2(int vm_fd);
int load_guest(void *memory_start, void *image_data, unsigned long image_size);
unsigned int KVM_GET_VCPU_MMAP_SIZE(int kvm_fd);
int run_vm(int vcpu_fd, int vcpu_map_size, void *mem);
int KVM_SET_CPUID2(int kvm_fd, int vcpu_fd);
