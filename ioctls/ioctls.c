#include <stdio.h>
#include <sys/ioctl.h>
#include <asm/ioctl.h>
#include <fcntl.h>

#include "./ioctls.h"

int KVM_GET_API_VERSION(int fd)
{
  __u_long req = _IO(KVM_ID, KVM_GET_API_VERSION_seq);
  return ioctl(fd, req, 0);
}

int KVM_CREATE_VM(int fd)
{
  __u_long req = _IO(KVM_ID, KVM_CREATE_VM_seq);
  return ioctl(fd, req, 0);
}

int KVM_SET_USER_MEMORY_REGION(int fd, struct kvm_userspace_memory_region region)
{
  __u_long req = _IOW(KVM_ID, KVM_SET_USER_MEMORY_REGION_seq, struct kvm_userspace_memory_region);
  return ioctl(fd, req, &region);
}

unsigned int KVM_CHECK_EXTENSION(int fd, int extension_identifier)
{
  __u_long req = _IO(KVM_ID, KVM_CHECK_EXTENSION_seq);
  return ioctl(fd, req, extension_identifier);
}

int KVM_CREATE_VCPU(int fd, unsigned int vcpu_id)
{
  __u_long req = _IO(KVM_ID, KVM_CREATE_VCPU_seq);
  return ioctl(fd, req, vcpu_id);
}

int KVM_GET_and_SET_SREGS(int vcpu, short arm64)
{
  if (arm64 == 0)
  {
    // get sregs
    struct kvm_sregs sregs;
    __u_long req = _IOR(KVM_ID, KVM_GET_SREGS_seq, struct kvm_sregs);
    int ret = ioctl(vcpu, req, &sregs);
    if (ret == -1)
    {
      return ret;
    }
    // init sregs
    sregs.cs.base = 0;
    sregs.cs.limit = ~0;
    sregs.cs.g = 1;

    sregs.ds.base = 0;
    sregs.ds.limit = ~0;
    sregs.ds.g = 1;

    sregs.fs.base = 0;
    sregs.fs.limit = ~0;
    sregs.fs.g = 1;

    sregs.gs.base = 0;
    sregs.gs.limit = ~0;
    sregs.gs.g = 1;

    sregs.es.base = 0;
    sregs.es.limit = ~0;
    sregs.es.g = 1;

    sregs.ss.base = 0;
    sregs.ss.limit = ~0;
    sregs.ss.g = 1;

    sregs.cs.db = 1;
    sregs.ss.db = 1;
    sregs.cr0 |= 1; // enables protected mode

    req = _IOW(KVM_ID, KVM_SET_SREGS_seq, struct kvm_sregs);
    ret = ioctl(vcpu, req, &sregs);
    return ret;
  }
  else
  {
    return -1;
  }
}

int KVM_SET_REGS(int vcpu)
{
  struct kvm_regs regs;
  regs.rflags = 2;
  regs.rip = 0x100000; // start of kernel
  regs.rsi = 0x10000;  // start of boot params
  __u_long req = _IOW(KVM_ID, KVM_SET_REGS_seq, struct kvm_regs);
  return ioctl(vcpu, req, &regs);
}
