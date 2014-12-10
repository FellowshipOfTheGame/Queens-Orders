// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/PlayerController.h"
#include "RTS_Controller.generated.h"

/**
 * 
 */
UCLASS()
class WARGAME_API ARTS_Controller : public APlayerController
{
	GENERATED_BODY()

public:

	// Constructions
	void setCurrentSelectedSlot(UObject *slot);


	// Actor interface
	virtual void BeginPlay() override;
	

private:
	UObject *currentBuildingSlot;

};
