# Specify the minimum version.
cmake_minimum_required(VERSION 3.22)

# Specify the project info.
project(KVM_IOCTLS VERSION 0.1.0 DESCRIPTION "Shared library for calling KVM ioctls from csharp")

#[[ Declare the library targets
 add c source and header for each function]]
add_library(${PROJECT_NAME} SHARED
    ioctls.c
    ioctls.h
)

# Configure the directories to search for header files.
target_include_directories(${PROJECT_NAME} PRIVATE ./)

# Set the version property.
set_target_properties(${PROJECT_NAME} PROPERTIES VERSION ${PROJECT_VERSION})

# Set the shared object version property to the project's major version.
set_target_properties(${PROJECT_NAME} PROPERTIES SOVERSION ${PROJECT_VERSION_MAJOR})

#[[ Set the public header property to the one with the actual API.
 For multiple, list like, "DIR;DIR;DIR;DIR;"]]
set_target_properties(${PROJECT_NAME} PROPERTIES PUBLIC_HEADER "ioctls.h;")

#[[  --- How do build ---  

  mkdir build
  cd build
  cmake ..
  make all

  done]]