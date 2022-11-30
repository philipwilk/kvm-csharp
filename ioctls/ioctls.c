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
  return ioctl(fd, req, region);
}