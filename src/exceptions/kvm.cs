
namespace Main
{
  public class vm_fd_not_created_Exception : SystemException
  {
    public vm_fd_not_created_Exception(int fd) : base(String.Format("Error while creating vm file descriptor: received file descriptor as {0} from kernel.", fd)) { }
  }

  public class user_memory_region_not_set_Exception : SystemException
  {
    public user_memory_region_not_set_Exception(nuint capacity, int vm_fd) : base(String.Format("Failed to set user memory region of {0} bytes on vm {1}.", capacity, vm_fd)) { }
  }

  public class not_enough_logical_processors : ArgumentOutOfRangeException
  {
    public not_enough_logical_processors(uint cores, int logicals) : base(String.Format("Requested vm with {0} cores, but only {1} logical processors available on host.", cores, logicals)) { }
  }

  public class failed_creating_vcpu : SystemException
  {
    public failed_creating_vcpu(uint core, int vm_fd) : base(String.Format("Failed to create core {0} for vm with fd {1}", core, vm_fd)) { }
  }
}