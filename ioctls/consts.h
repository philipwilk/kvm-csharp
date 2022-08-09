/*
  I think all executables need the naming convention lib(WHATEVER).so
  Then when importing it in csharp you import it as (WHATEVER).so
*/
// I know i could import <sys/kvm.h> but that would be too easy
// KVM ioctl id
const int KVM_ID = 0xAE;

// sequence numbers
const int KVM_GET_API_VERSION_seq = 0x00;