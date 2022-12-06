// I know i could import <sys/kvm.h> but that would be too easy
// KVM ioctl id
const int KVM_ID = 0xAE;

// sequence numbers
const int KVM_GET_API_VERSION_seq = 0x00;
const int KVM_CREATE_VM_seq = 0x01;
const int KVM_CHECK_EXTENSION_seq = 0x03;
const int KVM_CREATE_VCPU_seq = 0x41;
const int KVM_SET_USER_MEMORY_REGION_seq = 0x46;
const int KVM_SET_TSS_ADDR_seq = 0x47;
const int KVM_SET_IDENTITY_MAP_ADDR_seq = 0x48;
const int KVM_CREATE_IRQCHIP_seq = 0x60;
const int KVM_CREATE_PIT2_seq = 0x77;
const int KVM_SET_REGS_seq = 0x82;
const int KVM_GET_SREGS_seq = 0x83;
const int KVM_SET_SREGS_seq = 0x84;

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
int load_guest(unsigned long long memory_start, unsigned long long image_data, unsigned long image_size);
