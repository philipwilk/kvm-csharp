#include <stdio.h>
#include <sys/ioctl.h>
#include <asm/ioctl.h>
#include <fcntl.h>

#include "../consts.h"

int main()
{
  return 0;
}

int KVM_GET_API_VESION(int fd)
{
  __u_long req = _IO(KVM_ID, KVM_GET_API_VERSION_seq);
  return ioctl(fd, req, 0);
}
