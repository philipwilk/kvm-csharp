// I know i could import <sys/kvm.h> but that would be too easy
// KVM ioctl id
const int KVM_ID = 0xAE;

// sequence numbers
const int KVM_GET_API_VERSION_seq = 0x00;
const int KVM_CREATE_VM_seq = 0x01;
const int KVM_SET_USER_MEMORY_REGION_seq = 0x46;

// Structs
struct kvm_userspace_memory_region
{
  unsigned int slot;
  unsigned int flags;
  unsigned long long guest_phys_addr;
  unsigned long long memory_size;    /* bytes */
  unsigned long long userspace_addr; /* start of the userspace allocated memory */
};

int KVM_CREATE_VM(int fd);
int KVM_GET_API_VERSION(int fd);
int KVM_SET_USER_MEMORY_REGION(int fd, struct kvm_userspace_memory_region);