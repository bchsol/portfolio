pragma solidity ^0.8.0;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol"; // 경로의 ERC20.sol 파일을 불러옴

contract MyToken is ERC20{  // ERC20파일에 현재의 코드를 덮어씀
  uint public INITIAL_SUPPLY = 12000;

  constructor() public ERC20("My Token", "MT"){ //constructor는 스마트컨트랙트가 생성될 때 1번만 호출됨
    _mint(msg.sender, INITIAL_SUPPLY);
  }
}
