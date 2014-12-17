// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/PlayerController.h"

#include "BuildingSlot.h"

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
	UFUNCTION(BlueprintCallable, Category = "Buildings")
		void setCurrentSelectedSlot(ABuildingSlot *slot);

	UFUNCTION(BlueprintCallable, BlueprintPure, Category = "Buildings")
		ABuildingSlot* getCurrentSelectedSlot() const;


	// Actor interface
	virtual void BeginPlay() override;
	

private:
	ABuildingSlot *currentBuildingSlot;

};
