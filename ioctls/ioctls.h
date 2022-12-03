// I know i could import <sys/kvm.h> but that would be too easy
// KVM ioctl id
const int KVM_ID = 0xAE;

// sequence numbers
const int KVM_GET_API_VERSION_seq = 0x00;
const int KVM_CREATE_VM_seq = 0x01;
const int KVM_CHECK_EXTENSION_seq = 0x03;
const int KVM_CREATE_VCPU_seq = 0x41;
const int KVM_SET_USER_MEMORY_REGION_seq = 0x46;
const int KVM_GET_SREGS_seq = 0x83;

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

int KVM_CREATE_VM(int fd);
int KVM_GET_API_VERSION(int fd);
int KVM_SET_USER_MEMORY_REGION(int fd, struct kvm_userspace_memory_region);
unsigned int KVM_CHECK_EXTENSION(int fd, int extension_identifier);
int KVM_CREATE_VCPU(int fd, unsigned int vcpu_id);
int KVM_GET_SREGS(int vcpu, struct kvm_sregs sregs);