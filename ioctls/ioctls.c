#include <stdio.h>
#include <sys/ioctl.h>
#include <asm/ioctl.h>
#include <asm/bootparam.h>
#include <fcntl.h>

#include "./ioctls.h"

/// @brief Asks kvm what version it is.
/// @param fd Open file descriptor for /dev/kvm
/// @return Usually 12
int KVM_GET_API_VERSION(int fd)
{
  __u_long req = _IO(KVM_ID, KVM_GET_API_VERSION_seq);
  return ioctl(fd, req, 0);
}

/// @brief Creates a new vm in kvm, with no resources assigned to it.
/// @param fd Open file descriptor for /dev/kvm
/// @return File descriptor of new vm, or -1 on error.
int KVM_CREATE_VM(int fd)
{
  __u_long req = _IO(KVM_ID, KVM_CREATE_VM_seq);
  return ioctl(fd, req, 0);
}

/// @brief Assigns a memory region to a virtual machine using the vm file descriptor.
/// @param fd Open file descriptor for a virtual machine.
/// @param region kvm_userspace_memory_region struct that tells kvm how and what memory region to assign to this vm.
/// @return 0 on success, -1 on failure.
int KVM_SET_USER_MEMORY_REGION(int fd, struct kvm_userspace_memory_region region)
{
  __u_long req = _IOW(KVM_ID, KVM_SET_USER_MEMORY_REGION_seq, struct kvm_userspace_memory_region);
  return ioctl(fd, req, &region);
}

/// @brief Asks kvm whether it supports and extension or not.
/// @param fd Open file descriptor for /dev/kvm.
/// @param extension_identifier Sequence number for the ioctl that is being asked about.
/// @return 0 if supported, positive integer to provide additionl information, or -1 if unsupported
unsigned int KVM_CHECK_EXTENSION(int fd, int extension_identifier)
{
  __u_long req = _IO(KVM_ID, KVM_CHECK_EXTENSION_seq);
  return ioctl(fd, req, extension_identifier);
}

/// @brief Creates a vcpu for a vm with the provides vcpu id.
/// @param fd File descriptor for a vm.
/// @param vcpu_id Which vcpu this is creating.
/// @return 0 on success or -1 on failure.
int KVM_CREATE_VCPU(int fd, unsigned int vcpu_id)
{
  __u_long req = _IO(KVM_ID, KVM_CREATE_VCPU_seq);
  return ioctl(fd, req, vcpu_id);
}

/// @brief Assigns special registers and regular registers necessary to start the vm.
/// @param vcpu File descriptor for a virtual cpu.
/// @param arm64 Whether the virtual machine is runnining on the arm64 platform or not.
/// @return 0 on success, -1 on failure
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

/// @brief Assigns registers for a vcpu.
/// @param vcpu File descriptor for a virtual cpu.
/// @return 0 on success, -1 on failure.
int KVM_SET_REGS(int vcpu)
{
  struct kvm_regs regs;
  regs.rflags = 2;
  regs.rip = 0x100000; // start of kernel
  regs.rsi = 0x10000;  // start of boot params
  __u_long req = _IOW(KVM_ID, KVM_SET_REGS_seq, struct kvm_regs);
  return ioctl(vcpu, req, &regs);
}

/// @brief Intel virtualisation quirk awfulness.
/// @param vm_fd
/// @return
int KVM_SET_TSS_ADDR(int vm_fd)
{
  __u_long req = _IO(KVM_ID, KVM_SET_TSS_ADDR_seq);
  return ioctl(vm_fd, req, 0xfffbc000 + 0x1000); // numbers from qemu implementation
}

int KVM_SET_IDENTITY_MAP_ADDR(int vm_fd)
{
  __u_long req = _IOW(KVM_ID, KVM_SET_IDENTITY_MAP_ADDR_seq, unsigned long);
  unsigned long identity_base = 0xfeffc000;
  return ioctl(vm_fd, req, &identity_base);
}

int KVM_CREATE_IRQCHIP(int vm_fd)
{
  __u_long req = _IO(KVM_ID, KVM_CREATE_IRQCHIP_seq);
  return ioctl(vm_fd, req, 0);
}

int KVM_CREATE_PIT2(int vm_fd)
{
  __u_long req = _IOW(KVM_ID, KVM_CREATE_PIT2_seq, struct kvm_pit_config);
  struct kvm_pit_config pit;
  pit.flags = 0;
  return ioctl(vm_fd, req, &pit);
}
